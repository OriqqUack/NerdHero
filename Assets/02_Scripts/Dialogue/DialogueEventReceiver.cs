using System;
using Coffee.UIExtensions;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using Spine;
using Spine.Unity;
using Spine.Unity.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEventReceiver : MonoBehaviour
{
    [SerializeField] private Transform actor;
    [SerializeField] private Transform joystickTransform;
    [SerializeField] private Transform expSliderTransform;
    [SerializeField] private Transform skillContents;
    [SerializeField] private Transform rightIndicator;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button continueButton2;
    [SerializeField] private Image raycastImage;
    [SerializeField] private SkeletonGraphicAnimationHandle skeletonAnimation;
    [SerializeField] private GameObject clapEffect;
    [SerializeField] private Transform effectParent;

    [Space] 
    [SerializeField] private AnimationReferenceAsset blank;
    [SerializeField] private AnimationReferenceAsset mouse_Positive;
    [SerializeField] private AnimationReferenceAsset mouse_Nagative;
    [SerializeField] private AnimationReferenceAsset scenario4_1_loop;
    [SerializeField] private AnimationReferenceAsset scenario4_2;
    [SerializeField] private AnimationReferenceAsset scenario4_2_loop;
    [SerializeField] private AnimationReferenceAsset scenario5;
    [SerializeField] private AnimationReferenceAsset scenario5_loop;
    [SerializeField] private AnimationReferenceAsset scenario6;
    [SerializeField] private AnimationReferenceAsset scenario6_loop;
    [SerializeField] private AnimationReferenceAsset scenario7;
    [SerializeField] private AnimationReferenceAsset scenario7_loop;
    [SerializeField] private AnimationReferenceAsset scenario8_loop;
    [SerializeField] private AnimationReferenceAsset scenario9;
    [SerializeField] private AnimationReferenceAsset scenario9_loop;

    private Spine.AnimationState _animationState;

    private void Start()
    {
        _animationState = GetComponent<SkeletonGraphic>().AnimationState;
        _animationState.Event += HandleAnimationStateEventNonSkill;
    }

    public void ZoomInit()
    {
        Camera.main.GetComponent<CameraFollow>().ZoomInit();
    }

    public void ZoomIn()
    {
        Camera.main.GetComponent<CameraFollow>().ZoomIn(3f);
    }

    public void ActorOut()
    {
        actor.GetComponent<SkeletonGraphic>().enabled = false;
    }

    public void ActorIn()
    {
        actor.GetComponent<SkeletonGraphic>().enabled = true;
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
        ActorOut();
        SpotlightController.Instance.SetTarget(joystickTransform, null);
    }

    public void SpotlightExpSlider()
    {
        ActorOut();
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

    public void FourthAction()
    {
        PlayAnimation(scenario4_1_loop, 0);
        PlayAnimation(mouse_Nagative, 1);
    }

    public void FourthAction2()
    {
        PlayAnimations(scenario4_2, scenario4_2_loop, 0);
    }

    public void FifthAction()
    {
        PlayAnimations(scenario5, scenario5_loop, 0);
    }

    public void SixthAction()
    {
        ActorIn();
        SpotlightOff();
        PlayAnimations(scenario6, scenario6_loop, 0);
    }

    public void SeventhAction()
    {
        ActorIn();
        PlayAnimations(scenario7, scenario7_loop, 0);
    }

    public void EighthAction()
    {
        ActorIn();
        PlayAnimation(mouse_Positive , 1);
        PlayAnimation(scenario8_loop, 0);
    }

    public void NinthAction()
    {
        SpotlightOff();
        ActorIn();
        PlayAnimations(scenario6, scenario6_loop, 0);
    }

    public void TenthAction()
    {
        ActorIn();
        PlayAnimation(mouse_Nagative, 1);
        PlayAnimations(scenario7, scenario7_loop, 0);
    }

    public void EleventhAction()
    {
        PlayAnimation(mouse_Nagative, 1);
        PlayAnimations(scenario9, scenario9_loop, 0);
    }
    
    public void PlayAnimations(Spine.Animation event1, Spine.Animation event2, int index)
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            skeletonAnimation.PlayStartAndLoop(event1, event2, index);
        });
    }
    
    public void PlayOneShotAnimation(Spine.Animation event1, int index)
    {
        skeletonAnimation.PlayOneShot(event1, index);
    }

    public void PlayAnimation(Spine.Animation event1, int index)
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            skeletonAnimation.PlayAnimationForState(event1, index);
        });
    }
    
    public void DefaultMouse()
    {
        PlayAnimation(blank, 1);
    }
    
    private void HandleAnimationStateEventNonSkill(TrackEntry trackentry, Spine.Event e)
    {
        if (e.Data.Name == "tam effect")
        {
            CreateEffect();
        }
    }
    
    private void CreateEffect()
    {
        GameObject effect = Managers.Resource.Instantiate(clapEffect, effectParent);
        Managers.Resource.Destroy(effect, 2.0f);
    }
}