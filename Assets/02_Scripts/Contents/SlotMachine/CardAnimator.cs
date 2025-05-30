using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardAnimator : UiWindow
{
    public delegate void OnCardSpawned();
    public static OnCardSpawned onCardSpawned;
    public delegate void OnCardDelete();
    public static OnCardDelete onCardDelete;
    
    [UnderlineTitle("Card Setting")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Vector2 centerPos = new Vector2(0f, 100f); // 중앙 기준 위치
    [SerializeField] private Button resetButton;
    [SerializeField] private CardHolder cardHolder;
    [SerializeField] private GameObject levelEffect;
    [SerializeField] private List<CardProbabilityConfig> gradeProbConfigs;
    
    private GameObject[] _cards = new GameObject[3];
    private CardBase[] _cardBases;
    private RectTransform _canvasRectTransform;
    private CanvasGroup _resetCg;
    private float _canvasHeight;
    private float _canvasWidth;
    private CardSelector _cardSelector;
    private CardDatabase _cardData;
    private CardProbabilityManager _cardProbabilityManager;
    private Entity _entity;
    private int _currentLevel;
    private AttributeType _recentAttr = AttributeType.BaseAttack;
    private AttributeType _previousAttr = AttributeType.BaseAttack;
    
    public GameObject[] Cards => _cards;
    
    protected override void Start()
    {
        _entity = WaveManager.Instance.PlayerEntity;
        _canvasRectTransform = transform.GetComponentInParent<RectTransform>();
        _canvasWidth = _canvasRectTransform.rect.width;
        _canvasHeight = _canvasRectTransform.rect.height;
        resetButton.onClick.AddListener(() => ResetCard());
        _cardData = new CardDatabase(cardHolder);
        _cardProbabilityManager = new CardProbabilityManager(gradeProbConfigs);
        _cardSelector = new CardSelector(_entity, _cardData, _cardProbabilityManager);
        
        SpawnScrolls(2, true);
    }

    public void SpawnScrolls(int level, bool isFirst = false)
    {
        _currentLevel = level;
        _cardBases = _cardSelector.GetCardBases(level, _recentAttr, _previousAttr, isFirst);

        float scrollWidth = _canvasWidth * 0.25f;
        float scrollHeight = cardPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = scrollWidth * 1.1f;

        Sequence spawnSeq = DOTween.Sequence();

        for (int i = 0; i < 3; i++)
        {
            GameObject scroll = Instantiate(cardPrefab, transform);
            CardUI card = scroll.GetComponent<CardUI>();
            card.Setup(_cardBases[i].Effect);
            RectTransform rect = scroll.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(scrollWidth, scrollHeight);

            // 1. 튕기며 떨어지는 연출을 위한 시작 위치 설정
            rect.anchoredPosition = new Vector2(centerPos.x + (i - 1) * spacing, centerPos.y + 200f); // 위에서 시작

            // 2. 튕기며 떨어지는 연출 + 팝 효과 + Glow 효과 삽입
            spawnSeq.Insert(0.1f * i,
                rect.DOAnchorPosY(centerPos.y, 0.6f)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() =>
                    {
                        // 3. 팝 효과
                        rect.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 1);

                        // 4. 빛 번쩍 효과
                        if (card.overlayImage != null)
                        {
                            card.overlayImage.DOFade(0.8f, 0.1f)
                                .OnComplete(() => card.overlayImage.DOFade(0f, 0.3f));
                        }
                    })
            );

            _cards[i] = scroll;
        }

        // Reset 버튼 처리
        _resetCg = resetButton.GetComponent<CanvasGroup>();
        if (_resetCg == null)
        {
            _resetCg = resetButton.gameObject.AddComponent<CanvasGroup>();
        }

        resetButton.gameObject.SetActive(true);
        _resetCg.alpha = 0f;
        _resetCg.interactable = false;
        _resetCg.blocksRaycasts = false;

        spawnSeq.OnComplete(() =>
        {
            _resetCg.DOFade(1f, 0.2f).OnComplete(() =>
            {
                _resetCg.interactable = true;
                _resetCg.blocksRaycasts = true;
                for (int i = 0; i < _cards.Length; i++)
                {
                    int i1 = i;
                    _cards[i].GetComponent<Button>().onClick.AddListener(() => CardSelected(i1));
                }
            });
            onCardSpawned?.Invoke();
        });
    }
    
    /*public void SpawnCards(int level, bool isFirst = false)
    {
        _currentLevel = level;
        _cardBases = _cardSelector.GetCardBases(level, _recentAttr, _previousAttr, isFirst);
        
        float cardWidth = _canvasWidth * 0.25f; // 예: 전체 너비의 25%
        float cardHeight = cardPrefab.GetComponent<RectTransform>().sizeDelta.y;

        float spacing = cardWidth * 1.1f;

        Sequence spawnSeq = DOTween.Sequence();

        for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
            card.GetComponent<CardUI>().Setup(_cardBases[i].Effect);
            RectTransform rect = card.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(cardWidth, cardHeight);
            rect.anchoredPosition = new Vector2(0, -_canvasHeight);

            float targetX = centerPos.x + (i - 1) * spacing;
            Vector2 targetPos = new Vector2(targetX, centerPos.y);

            // 카드 등장 애니메이션
            spawnSeq.Insert(0.2f * i, rect.DOAnchorPos(targetPos, 0.6f).SetEase(Ease.OutBack));

            int i1 = i;
            card.GetComponent<Button>().onClick.AddListener(() => FlipCard(card, i1));

            _cards[i] = card;
        }

        // ResetButton을 애니메이션 후 활성화 + 서서히 보이기
        _resetCg = resetButton.GetComponent<CanvasGroup>();
        if (_resetCg == null)
        {
            _resetCg = resetButton.gameObject.AddComponent<CanvasGroup>();
        }

        resetButton.gameObject.SetActive(true);
        _resetCg.alpha = 0f;
        _resetCg.interactable = false;
        _resetCg.blocksRaycasts = false;

        spawnSeq.OnComplete(() =>
        {
            _resetCg.DOFade(1f, 0.5f).OnComplete(() =>
            {
                _resetCg.interactable = true;
                _resetCg.blocksRaycasts = true;
            });
            onCardSpawned?.Invoke();
        });
    }*/

    /*private void FlipCard(GameObject card, int index)
    {
        card.GetComponent<Button>().interactable = false;
        Transform front = card.transform.Find("CardFront");
        Transform back = card.transform.Find("CardBack");

        Sequence flipSeq = DOTween.Sequence();

        // 1단계: 0 → 90도 회전 (오른쪽에서 왼쪽으로 돌아감)
        flipSeq.Append(card.transform.DORotate(new Vector3(0, 90f, 0), 0.25f, RotateMode.Fast))
            .SetEase(Ease.InOutSine)
            .AppendCallback(() =>
            {
                // 카드 면 뒤집기
                front.gameObject.SetActive(false);
                back.gameObject.SetActive(true);
            })
            // 2단계: 90 → 180도 회전 마저
            .Append(card.transform.DORotate(new Vector3(0, 180f, 0), 0.25f, RotateMode.Fast))
            .SetEase(Ease.InOutSine);
        
        _backBtn = back.Find("Button").GetComponent<Button>();
        _backBtn.onClick.AddListener(() => CardSelected(index));
    }*/

    private void CardSelected(int selectedIndex)
    {
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        
        _cards[selectedIndex].GetComponent<Button>().interactable = false;
        
        AttributeType selectedAttr = _cardBases[selectedIndex].attributeType;
        _previousAttr = _recentAttr;
        _recentAttr = selectedAttr;
        
        var clone = _cardBases[selectedIndex].Clone() as CardBase;
        clone.ApplyEffect();

        _resetCg.alpha = 0f;
        for (int i = 0; i < _cards.Length; i++)
        {
            GameObject card = _cards[i];

            if (i == selectedIndex)
            {
                if (card != null && card.GetComponent<RectTransform>() != null)
                {
                    RectTransform rect = card.GetComponent<RectTransform>();

                    Sequence seq = DOTween.Sequence();
                    seq.AppendInterval(0.3f)
                        .Append(rect.DOAnchorPosY(-_canvasHeight, 0.3f)
                            .SetEase(Ease.InBack)
                            .SetUpdate(true))
                        .OnComplete(() =>
                        {
                            CloseUI();
                        })
                        .OnKill(() =>
                        {
                            CloseUI();
                        });

                }
            }
            else
            {
                // 나머지 카드: CanvasGroup으로 알파값 줄이기
                CanvasGroup cg = card.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = card.AddComponent<CanvasGroup>();

                cg.DOFade(0f, 0.2f).SetEase(Ease.InOutQuad);
            }
        }
    }
    
    private void ResetCard()
    {
        Sequence resetSeq = DOTween.Sequence();

        for (int i = 0; i < _cards.Length; i++)
        {
            GameObject card = _cards[i];
            if (card == null) continue;

            RectTransform rect = card.GetComponent<RectTransform>();

            // 내려가며 사라지기
            resetSeq.Join(
                rect.DOAnchorPosY(-_canvasHeight, 0.4f)
                    .SetEase(Ease.InBack)
            );

            // 알파값도 줄이기
            CanvasGroup cg = card.GetComponent<CanvasGroup>();
            if (cg == null) cg = card.AddComponent<CanvasGroup>();
            resetSeq.Join(cg.DOFade(0f, 0.4f));
        }

        resetSeq.OnComplete(() =>
        {
            // 카드 전부 제거
            for (int i = 0; i < _cards.Length; i++)
            {
                if (_cards[i] != null)
                    Destroy(_cards[i]);
            }

            // 배열 초기화
            _cards = new GameObject[3];

            // 다시 생성
            SpawnScrolls(_currentLevel);
        });
    }

    private void SpawnEffect()
    {
        GameObject go = Managers.Resource.Instantiate(levelEffect);
        go.transform.position = _entity.transform.position;
        go.transform.SetParent(_entity.transform);
    }
    
    private void CloseUI()
    {
        Time.timeScale = 1f;
        if(_closeCallback != null) _closeCallback(_windowHolder);
        SpawnEffect();
        onCardDelete?.Invoke();
        Destroy(gameObject);
    }
}