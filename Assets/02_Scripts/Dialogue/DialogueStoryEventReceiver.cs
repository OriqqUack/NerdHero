using System;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using Spine;
using Spine.Unity;
using Spine.Unity.Examples;
using TMPro;
using UnityEngine;
//행동 0, 표정 1, 입모양 2
public class DialogueStoryEventReceiver : MonoBehaviour
{
    [SerializeField] private Transform actor;
    [SerializeField] private SkeletonGraphicAnimationHandle skeletonAnimation;
    [SerializeField] private TextMeshProUGUI textMesh;

    [Space]
    [SerializeField] private GameObject[] bgs;
    [SerializeField] private string[] convenrsationList;
    [SerializeField] private AudioClip[] audioClips;

    [Space] 
    [SerializeField] private AnimationReferenceAsset blank;
    [SerializeField] private AnimationReferenceAsset mouse_Positive;
    [SerializeField] private AnimationReferenceAsset mouse_Nagative;
    [SerializeField] private AnimationReferenceAsset scenario1;
    [SerializeField] private AnimationReferenceAsset scenario1_loop;
    [SerializeField] private AnimationReferenceAsset scenario2;
    [SerializeField] private AnimationReferenceAsset scenario2_loop;
    [SerializeField] private AnimationReferenceAsset scenario3;
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
    
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 1f;
    [SerializeField] private int vibrato = 10; // 흔들림 횟수
    [SerializeField] private float randomness = 90f;
    [SerializeField] private Transform panelTransform;

    private Tween shakeTween;
    
    private int _currentBg;
    private int _currentCon;
    
    private void Start()
    {
        DialogueManager.instance.conversationEnded += FadeIn;
        DialogueManager.instance.conversationStarted += BGChange;
    }

    public void FadeIn(Transform actor)
    {
        SceneTransitioner.Instance.FadeIn(() => NextConversation());
        textMesh.text = "";
    }

    public void FadeOut(Transform actor)
    {
        SceneTransitioner.Instance.FadeOut(null);
    }

    private void NextConversation()
    {
        _currentCon++;
        if (_currentCon >= convenrsationList.Length)
        {
            DialogueManager.instance.conversationEnded -= FadeIn;
            return;
        }
        DialogueManager.StartConversation(convenrsationList[_currentCon], actor);
        SceneTransitioner.Instance.FadeOut(null);
    }
    
    public void BGChange(Transform actor)
    {
        bgs[_currentBg].SetActive(false);
        _currentBg++;
        if (_currentBg >= bgs.Length) return;
        bgs[_currentBg].SetActive(true);
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
        skeletonAnimation.PlayAnimationForState(event1, index);
    }

    public void FirstAction()
    {
        Managers.SoundManager.Play(audioClips[0], Sound.Bgm);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            PlayAnimation(mouse_Positive, 1);
            PlayAnimations(scenario1, scenario1_loop, 0);
        });
    }

    public void SecondAction()
    {
        Managers.SoundManager.Play(audioClips[1], Sound.Bgm);
        DefaultAction();
        DOVirtual.DelayedCall(0.1f, () =>
        {
            PlayAnimation(mouse_Positive, 1);
            PlayAnimations(scenario2, scenario2_loop, 0);
        });
    }

    public void ThirdAction()
    {
        DefaultAction();
        DialogueManager.instance.conversationEnded -= FadeIn;
        DOVirtual.DelayedCall(0.1f, () =>
        {
            StartCameraShake();
            skeletonAnimation.skeletonAnimation.AnimationState.SetAnimation(0, scenario3, false);
            DialogueManager.instance.conversationEnded += transform1 =>
            {
                SceneTransitioner.Instance.StartTransitioning(SceneType.TutorialScene);
            };
        });
    }

    
    private void DefaultAction()
    {
        PlayAnimation(blank, 0);
        PlayAnimation(blank, 1);
    }

    public void DefaultMouse()
    {
        PlayAnimation(blank, 1);
    }

    public void StartCameraShake()
    {
        if (shakeTween != null && shakeTween.IsActive()) shakeTween.Kill();

        shakeTween = panelTransform.DOShakeRotation(
            duration: shakeDuration,
            strength: shakeStrength,
            vibrato: vibrato,
            randomness: randomness,
            fadeOut: true
        );
    }
    
}
