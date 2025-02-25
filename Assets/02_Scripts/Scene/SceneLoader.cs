using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float minLoadingTime = 3f;

    private void Start()
    {
        StartCoroutine(LoadAsync());
    }

    private IEnumerator LoadAsync()
    {
        string sceneToLoad = SceneTransitionManager.NextSceneName;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        
        //로딩이 완료되도 바로 넘어가지 않게
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;
            loadingSlider.value = Mathf.Clamp01((operation.progress + timer) / (0.9f + minLoadingTime));
            
            if (loadingSlider.value >= 1f)
            {
                operation.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
}
