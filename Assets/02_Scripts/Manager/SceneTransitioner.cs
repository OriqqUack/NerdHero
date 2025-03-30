using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum SceneType
{
    MainScene,
    InGameScene,
    LoadingScene
}

public class SceneTransitioner : MonoSingleton<SceneTransitioner>
{
    [SerializeField] private Image transitionImage;
    [SerializeField] private float lerpDuration = 1f; // 몇 초 동안 변화할지

    private static string referenceName = "_Lerp"; 
    
    private float _elapsedTime = 0f;
    private Material _mat;
    private bool _isStartScene = true;
    private void Start()
    {
        if (transitionImage != null)
        {
            _mat = new Material(transitionImage.material);
            transitionImage.material = _mat;
        }    
        
        StartCoroutine(SceneTransition(SceneType.MainScene, 0, 1));
    }

    public void StartTransitioning(SceneType sceneType, int currentLerp, int targetLerp)
    {
        StartCoroutine(SceneTransition(sceneType, currentLerp, targetLerp));
    }

    IEnumerator SceneTransition(SceneType sceneType, int currentLerp, int targetLerp)
    {
        while (true)
        {
            if (_elapsedTime < lerpDuration)
            {
                _elapsedTime += Time.deltaTime;

                // 0 → 1로 Lerp
                float t = _elapsedTime / lerpDuration;
                float lerpedValue = Mathf.Lerp(currentLerp, targetLerp, t);

                _mat.SetFloat(referenceName, lerpedValue);
            }
            else
            {
                _mat.SetFloat(referenceName, targetLerp);
                break;
            }
            yield return null;
        }
        _elapsedTime = 0;
        
        if(!_isStartScene)
            TransitionToScene(sceneType);
        
        _isStartScene = false;
    }
    
    private void TransitionToScene(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.MainScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_InGame");
                break;
            case SceneType.InGameScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_InGame");
                break;
            case SceneType.LoadingScene:
                SceneTransitionManager.LoadSceneInstantly("Scene_Loading");
                break;
        }
    }
}
