using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using PixelCrushers.DialogueSystem;

public enum SceneType
{
    CurrentScene,
    StartScene,
    StoryScene,
    TutorialScene,
    MainScene,
    InGameScene,
    LoadingScene
}

public class SceneTransitioner : MonoSingleton<SceneTransitioner>
{
    [SerializeField] private SceneType sceneType;
    [SerializeField] private Image transitionImage;
    [SerializeField] private float fadeDuration = 1f; // 페이드 인/아웃 시간
    
    private static readonly string referenceName = "_Lerp";
    private Material _mat;
    private bool _isTransitioning = false;
    private SceneType _nextSceneType;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Canvas를 
    }

    private void Start()
    {
        if (transitionImage != null)
        {
            _mat = new Material(transitionImage.material);
            transitionImage.material = _mat;
        }

        if (sceneType != SceneType.StartScene)
        {
            // 첫 로딩 때는 FadeOut 해서 서서히 보이게
            FadeOut(null);
        }
    }

    public void StartTransitioning(SceneType nextSceneType)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        _nextSceneType = nextSceneType;

        FadeIn(() =>
        {
            // FadeIn이 끝나면 LoadScene
            LoadScene(_nextSceneType);
            // 그리고 SceneLoaded 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        });
    }

    public void FadeIn(Action onComplete)
    {
        float value = 1f;

        DOTween.To(() => value,
            x => {
                value = x;
                _mat.SetFloat(referenceName, value);
            },
            0f,
            fadeDuration)
        .SetUpdate(true)
        .OnComplete(() =>
        {
            _mat.SetFloat(referenceName, 0f);
            onComplete?.Invoke();
        });
    }

    public void FadeOut(Action onComplete)
    {
        float value = 0f;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            DOTween.To(() => value,
                    x => {
                        value = x;
                        _mat.SetFloat(referenceName, value);
                    },
                    1f,
                    fadeDuration)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _mat.SetFloat(referenceName, 1f);
                    _isTransitioning = false;
                    SceneManager.sceneLoaded -= OnSceneLoaded; // FadeOut 끝났으면 이벤트 제거
                    onComplete?.Invoke();
                });
        });
    }

    private void LoadScene(SceneType nextSceneType)
    {
        switch (nextSceneType)
        {
            case SceneType.StartScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_Start");
                break;
            case SceneType.StoryScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_Story");
                break;
            case SceneType.TutorialScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_Tutorial");
                break;
            case SceneType.MainScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_Main");
                break;
            case SceneType.InGameScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_InGame");
                break;
            case SceneType.LoadingScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_Loading");
                break;
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새 Scene이 완전히 로드된 순간, FadeOut 시작
        FadeOut(null);
    }
}
