using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartScene : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button panel;
    [SerializeField] private GameObject alertText;

    [Space(10)] [Header("Text Floating")]
    [SerializeField] private float height = 1.0f;
    [SerializeField] private float duration = 1.0f;
    private Vector3 _initialPos;    
    
    private void Start()
    {
        panel.onClick.AddListener(() => OnClickPanel());
        TextFloating();
    }

    private void TextFloating()
    {
        _initialPos = alertText.transform.position;
        alertText.transform.DOMoveY(_initialPos.y + height, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnClickPanel()
    {
        SceneTransitionManager.LoadScene("Scene_Main");
    }
}
