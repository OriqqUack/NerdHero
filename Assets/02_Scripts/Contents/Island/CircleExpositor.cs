using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//Island 선택 스크립트
public class CircleExpositor : MonoSingleton<CircleExpositor>, ISaveable
{
    [SerializeField] private float radius = 40f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private Button enterButton; 

    private Transform[] _items;
    private List<Island> _islands = new List<Island>();

    private int _count = 0;
    private int _currentTarget = 0;
    private float _offsetRotation, _iniY;
    private float _zOffset = 0f;
    private Vector2 _touchStartPos;
    private bool _swiped = false;
    private Vector3[] _targetPositions;
    
    public int CurrentTargetIndex => _currentTarget;
    public Island CurrentIsland => _islands[_currentTarget];

    private void Start()
    {
        if(GameManager.Instance.CurrentIslandIndex == -1)
            _currentTarget = 0;
        else
            _currentTarget = GameManager.Instance.CurrentIslandIndex;
        
        _iniY = transform.position.y;
        
        _offsetRotation = 360.0f / _count;
        for (int i = 0; i < _count; i++)
        {
            float xPos = (i - _currentTarget) * 100f; // 간격 조정 가능
            Vector3 newPos = new Vector3(xPos, _iniY, 0f);
            _items[i].position = newPos;
        }

        _zOffset = radius - 40f;
        transform.position = new Vector3(transform.position.x, transform.position.y, _zOffset);
        
        if (GameManager.Instance.IsClear)
        {
            StartCoroutine(ScrollingCoroutine());
        }
        else
        {
            SetTarget(_currentTarget);
        }
    }

    private IEnumerator ScrollingCoroutine()
    {
        yield return new WaitForSeconds(1f);

        int index = GameManager.Instance.CurrentIslandIndex + 1;
        _currentTarget = index;
        if (!_islands.IsValidIndex(index)) yield break;
        
        if (_islands[index].IsLocked)
        {
            SetTarget(index);
            _islands[_currentTarget].LockedEffect.ShakeAndBreak(() =>
            {
                GameManager.Instance.IsClear = false;
                List<bool> clearedMapList = new List<bool>();
                foreach (var island in _islands)
                {
                    clearedMapList.Add(island.IsLocked);
                }
                Managers.BackendManager.UpdateField("clearedMap", clearedMapList);
            });
        }
        
        yield return null;
    }

    private void Update()
    {
#if UNITY_ANDROID
        HandleTouchInput();
#endif
    }

    public void ChangeTarget(int offset)
    {
        _currentTarget += offset;
        if (_currentTarget > _items.Length - 1) _currentTarget = 0;
        else if (_currentTarget < 0) _currentTarget = _items.Length - 1;

        UpdateItemPositions();
        EnterButtonActive(!_islands[_currentTarget].IsLocked);
    }

    public void SetTarget(int index)
    {
        _currentTarget = Mathf.Clamp(index, 0, _items.Length - 1);
        UpdateItemPositions();
        EnterButtonActive(!_islands[_currentTarget].IsLocked);
    }

    private void UpdateItemPositions()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            float xPos = (i - _currentTarget) * 100f;
            Vector3 targetPos = new Vector3(xPos, _iniY, 0f);

            // 이전 트윈이 있다면 중복 방지
            _items[i].DOKill(); 

            // DOTween으로 부드럽게 이동 (0.4초 동안 EaseOut)
            _items[i].DOMove(targetPos, 0.4f).SetEase(Ease.OutCubic);
        }
    }

    public void EnterButtonActive(bool active)
    {
        enterButton.interactable = active;
        if(active)
            enterButton.image.color = Color.white;
        else
            enterButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    public void Save(GameData data)
    {
        /*List<bool> isLocked = new List<bool>();
        foreach (Island island in _islands)
        {
            isLocked.Add(!island.IsLocked);
        }

        data.Map.clearedMap = new List<bool>(isLocked);*/
    }

    public void Load(GameData data)
    {
        _items = new Transform[transform.childCount];

        foreach (Transform child in transform)
        {
            _islands.Add(child.GetComponent<Island>());
            _items[_count] = child;
            _count++;
        }

        for (int i = 0; i < data.clearedMap.Count; i++)
        {
            _islands[i].IsLocked = data.clearedMap[i];
        }
    }
    
    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _touchStartPos = touch.position;
                _swiped = false;
                break;

            case TouchPhase.Moved:
                if (_swiped) return; // 한 번만 처리

                float deltaX = touch.position.x - _touchStartPos.x;

                if (Mathf.Abs(deltaX) > 100f) // 스와이프 감지 임계값 (조절 가능)
                {
                    if (deltaX > 0)
                    {
                        ChangeTarget(-1); // 오른쪽으로 스와이프 → 왼쪽 이동
                    }
                    else
                    {
                        ChangeTarget(1); // 왼쪽으로 스와이프 → 오른쪽 이동
                    }

                    _swiped = true; // 중복 감지 방지
                }
                break;

            case TouchPhase.Ended:
                _swiped = false;
                break;
        }
    }

}
