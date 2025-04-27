using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueEventReceiver : MonoBehaviour
{
    [SerializeField] private Transform joystickTransform;
    [SerializeField] private Transform expSliderTransform;
    [SerializeField] private Transform skillContents;
    [SerializeField] private Transform rightIndicator;
    [SerializeField] private Transform waveCountSlider;
    
    public void ZoomInit()
    {
        Camera.main.GetComponent<CameraFollow>().ZoomInit();
    }

    public void SpotlightOff()
    {
        SpotlightController.Instance.OffSpotlight();
    }

    public void SpotlightJoystick()
    {
        SpotlightController.Instance.SetTarget(joystickTransform, null);
    }

    public void SpotlightExpSlider()
    {
        SpotlightController.Instance.SetTarget(expSliderTransform, null);
    }

    public void SpotlightSkillBtn()
    {
        var btn = skillContents.Find("Btn_Skill2(Clone)");
        SpotlightController.Instance.SetTarget(expSliderTransform, btn);
    }

    public void SpotlightIndicator()
    {
        SpotlightController.Instance.SetTarget(rightIndicator, null);
    }

    public void SpotlightWaveCountSlider()
    {
        SpotlightController.Instance.SetTarget(waveCountSlider, null);
    }
}