using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_HUD_Indicator : MonoBehaviour
{
    [SerializeField] private GameObject leftIndicator;
    [SerializeField] private TextMeshProUGUI leftIndicatorText;
    [SerializeField] private GameObject rightIndicator;
    [SerializeField] private TextMeshProUGUI rightIndicatorText;
    
    private Camera cam;
    private int _leftCount;
    private int _rightCount;
    
    private List<Coroutine> _coroutines = new List<Coroutine>();
    
    private void Start()
    {
        WaveManager.Instance.OnEnemySpawned += IndicatorUpdate;
        cam = Camera.main;
        
        leftIndicator.transform.DOScale(Vector3.one * 1.2f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        rightIndicator.transform.DOScale(Vector3.one * 1.2f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
        leftIndicator.SetActive(false);
        rightIndicator.SetActive(false);
    }

    private void IndicatorUpdate(List<Entity> entities)
    {
        _leftCount = 0;
        _rightCount = 0;
        
        foreach (var entity in entities)
        {
            Vector3 viewportPos = cam.WorldToViewportPoint(entity.transform.position);
            bool isOffscreen = (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1);

            if (isOffscreen)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(entity.transform.position);
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

                bool isLeft = screenPos.x < screenCenter.x;
                if(isLeft) _leftCount++; else _rightCount++; ;
            }
        }

        leftIndicator.SetActive(false);
        rightIndicator.SetActive(false);
        
        if (_leftCount > 0)
        {
            leftIndicatorText.text = _leftCount.ToString();
            _coroutines.Add(StartCoroutine(ActiveGameObject(leftIndicator)));
        }

        if (_rightCount > 0)
        {
            rightIndicatorText.text = _rightCount.ToString();
            _coroutines.Add(StartCoroutine(ActiveGameObject(rightIndicator)));
        }
    }

    IEnumerator ActiveGameObject(GameObject go)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(3f);
        go.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (var coroutine in _coroutines)
        {
            StopCoroutine(coroutine);
        }
    }
}
