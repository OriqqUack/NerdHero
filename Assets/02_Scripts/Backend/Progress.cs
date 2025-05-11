using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    [SerializeField] private Slider sliderProgress;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private float progressTime;
    [SerializeField] private StartSceneBgController bgController;

    public void Play(UnityAction action = null)
    {
        StartCoroutine(OnProgress(action));
    }

    private IEnumerator OnProgress(UnityAction action)
    {
        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / progressTime;
            
            progressText.text = $"Now Loading.... {sliderProgress.value * 100 :F0}%";
            sliderProgress.value = Mathf.Lerp(0, 1, percent);
            yield return null;
        }
        
        bgController.Setup();
        action?.Invoke();
    }
}
