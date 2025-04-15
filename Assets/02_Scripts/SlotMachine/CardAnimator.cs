using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardAnimator : UiWindow
{
    [UnderlineTitle("Card Setting")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Vector2 centerPos = new Vector2(0f, 100f); // 중앙 기준 위치
    [SerializeField] private Button resetButton;

    private GameObject[] _cards = new GameObject[3];
    private RectTransform _canvasRectTransform;
    private CanvasGroup _resetCg;
    private float _canvasHeight;
    private float _canvasWidth;
    protected override void Start()
    {
        _canvasRectTransform = transform.GetComponentInParent<RectTransform>();
        _canvasWidth = _canvasRectTransform.rect.width;
        _canvasHeight = _canvasRectTransform.rect.height;
        resetButton.onClick.AddListener(() => ResetCard());
        SpawnCards();
    }

    private void SpawnCards()
    {
        float cardWidth = _canvasWidth * 0.25f; // 예: 전체 너비의 25%
        float cardHeight = cardPrefab.GetComponent<RectTransform>().sizeDelta.y;

        float spacing = cardWidth * 1.1f;

        Sequence spawnSeq = DOTween.Sequence();

        for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform);
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
        });
    }

    private void FlipCard(GameObject card, int index)
    {
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
        
        back.Find("Button").GetComponent<Button>().onClick.AddListener(() => CardSelected(index));
    }

    private void CardSelected(int selectedIndex)
    {
        _resetCg.alpha = 0f;
        for (int i = 0; i < _cards.Length; i++)
        {
            GameObject card = _cards[i];

            if (i == selectedIndex)
            {
                // 선택된 카드: 확대 + 강조
                card.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack);

                // 카드가 내려가는 애니메이션
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(0.6f) // 나머지 카드 사라지는 시간 기다림
                    .Append(card.GetComponent<RectTransform>().DOAnchorPosY(-_canvasHeight, 0.6f)
                        .SetEase(Ease.InBack)).OnComplete(CloseUI);
            }
            else
            {
                // 나머지 카드: CanvasGroup으로 알파값 줄이기
                CanvasGroup cg = card.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = card.AddComponent<CanvasGroup>();

                cg.DOFade(0f, 0.5f).SetEase(Ease.InOutQuad);
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
            SpawnCards();
        });
    }

    private void CloseUI()
    {
        Time.timeScale = 1f;
        if(_closeCallback != null) _closeCallback(_windowHolder);
        Destroy(gameObject);
    }
}