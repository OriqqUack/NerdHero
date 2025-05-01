using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEventReceiver : MonoBehaviour
{
    [SerializeField] private Transform joystickTransform;
    [SerializeField] private Transform expSliderTransform;
    [SerializeField] private Transform skillContents;
    [SerializeField] private Transform rightIndicator;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button continueButton2;
    [SerializeField] private Image raycastImage;
    public void ZoomInit()
    {
        Camera.main.GetComponent<CameraFollow>().ZoomInit();
    }

    public void ZoomIn()
    {
        Camera.main.GetComponent<CameraFollow>().ZoomIn(3f);
    }
    
    public void SpotlightOff()
    {
        SpotlightController.Instance.OffSpotlight();
    }

    public void SpotlightOffAndTime()
    {
        SpotlightOff();
        Time.timeScale = 1;
        continueButton2.interactable = false;
        continueButton.interactable = false;
        raycastImage.raycastTarget = false;
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

    public void SpotlightSkillBtnIgnoreRayCast()
    {
        continueButton2.interactable = false;
        continueButton.interactable = false;
        raycastImage.raycastTarget = false;
        var btn = skillContents.Find("Btn_Skill2(Clone)");
        btn.AddComponent<OnSkillButtonClicked>();
        SpotlightController.Instance.SetTarget(null, btn);
        DialogueManager.instance.conversationEnded += EndSkillConversation;
    }
    
    public void ButtonInteractableTrue()
    {
        continueButton2.interactable = false;
        continueButton.interactable = false;
        raycastImage.raycastTarget = false;
    }

    public void EndSkillConversation(Transform t)
    {
        continueButton2.interactable = true;
        continueButton.interactable = true;
        raycastImage.raycastTarget = true;
        DialogueManager.instance.conversationEnded -= EndSkillConversation;
    }

    public void SpotlightIndicator()
    {
        SpotlightController.Instance.SetTarget(rightIndicator, null);
    }
}