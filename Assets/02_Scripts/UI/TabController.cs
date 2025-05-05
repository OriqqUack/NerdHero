using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening; 

public class TabController : MonoSingleton<TabController>
{
    public List<Tab> tabs;      // 탭들
    public float widthOffset = 20f;
    public float heightOffset = 20f;
    public float tweenDuration = 0.3f;    

    private RectTransform containerRect;
    private float normalHeight;
    private float normalWidth;
    private float expandedHeight;
    private float expandedWidth;
    private List<RectTransform> _rectTabs = new List<RectTransform>();
    void Awake()
    {
        containerRect = GetComponent<RectTransform>();

        foreach (Tab tab in tabs)
        {
            _rectTabs.Add(tab.transform as RectTransform);
        }
        
        if (tabs.Count > 0)
        {
            // 초기 크기 측정
            normalHeight = _rectTabs[0].rect.height;
            expandedHeight = normalHeight + heightOffset;
            normalWidth = _rectTabs[0].rect.width;
            expandedWidth = normalWidth + widthOffset;
        }
    }

    public void OnTabClicked(int index)
    {
        float totalWidth = containerRect.rect.width;
        int tabCount = tabs.Count;

        if (tabCount <= 1) return;

        float collapsedWidth = (totalWidth - expandedWidth) / (tabCount - 1);

        for (int i = 0; i < _rectTabs.Count; i++)
        {
            if (i == index)
            {
                tabs[i].Setup();
            }
            else
            {
                tabs[i].Reset();
            }
            float targetWidth = (i == index) ? expandedWidth : collapsedWidth;
            float targetHeight = (i == index) ? expandedHeight : normalHeight;

            // ✅ DOTween으로 부드럽게 크기 조정
            _rectTabs[i].DOSizeDelta(new Vector2(targetWidth, targetHeight), tweenDuration).SetEase(Ease.OutQuad);
        }
    }
    
    public void ResetTabs()
    {
        float totalWidth = containerRect.rect.width;
        int tabCount = tabs.Count;

        if (tabCount == 0) return;

        float equalWidth = totalWidth / tabCount;

        for (int i = 0; i < _rectTabs.Count; i++)
        {
            _rectTabs[i].DOSizeDelta(new Vector2(equalWidth, normalHeight), tweenDuration).SetEase(Ease.OutQuad);
            tabs[i].Reset();
        }
    }
}