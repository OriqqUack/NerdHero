using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CircleExpositor : MonoSingleton<CircleExpositor>
{
    [SerializeField] private float radius = 40f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private Button enterButton; 

    private Transform[] _items;
    private List<Island> _islands = new List<Island>();
    private Quaternion _dummyRotation;

    private int _count = 0;
    private int _currentTarget = 0;
    private float _offsetRotation, _iniY;
    private float _zOffset = 0f;
    
    public int CurrentTargetIndex => _currentTarget;
    public Island CurrentIsland => _islands[_currentTarget];

    private void Start()
    {
        _currentTarget = GameManager.Instance.CurrentIslandIndex;
        _dummyRotation = transform.rotation;
        _iniY = transform.position.y;

        _items = new Transform[transform.childCount];
        foreach (Transform child in transform)
        {
            _islands.Add(child.GetComponent<Island>());
            _items[_count] = child;
            _count++;
        }

        _offsetRotation = 360.0f / _count;
        for (int i = 0; i < _count; i++)
        {
            float angle = i * Mathf.PI * 2f / _count;
            Vector3 newPos = new Vector3(Mathf.Sin(angle) * radius, _iniY, -Mathf.Cos(angle) * radius);
            _items[i].position = newPos;
        }

        _zOffset = radius - 40f;
        transform.position = new Vector3(transform.position.x, transform.position.y, _zOffset);

        if (GameManager.Instance.IsClear)
        {
            StartCoroutine(ScrollingCoroutine());
        }
    }

    private IEnumerator ScrollingCoroutine()
    {
        yield return new WaitForSeconds(1f);
        
        int index = GameManager.Instance.CurrentIslandIndex + 1;
        if (!_islands.IsValidIndex(index)) yield break;
        
        ChangeTarget(index);
        if (_islands[_currentTarget].IsLocked)
        {
            _islands[_currentTarget].LockedEffect.ShakeAndBreak();
        }
        
        yield return null;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _dummyRotation, rotateSpeed * Time.deltaTime);
    }

    public void ChangeTarget(int offset)
    {
        _currentTarget += offset;
        if (_currentTarget > _items.Length - 1) _currentTarget = 0;
        else if (_currentTarget < 0) _currentTarget = _items.Length - 1;
        _dummyRotation *= Quaternion.Euler(Vector3.up * (offset * _offsetRotation));

        if (_islands[_currentTarget].IsLocked)
        {
            EnterButtonActive(false);
        }
        else
        {
            EnterButtonActive(true);
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
}
