using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

//Intro Image 컨트롤
public class StartSceneBgController : MonoBehaviour
{
    [SerializeField] private Image nightImage;
    [SerializeField] private SkeletonGraphic light1;
    [SerializeField] private SkeletonGraphic light2;
    [SerializeField] private Volume volume;
    [SerializeField] private Button loadButton;

    [SerializeField] private float fadeDuration;
    private Color originalColor;
    
    public void Setup()
    {
        FadeAlphaSprite(nightImage, 1, fadeDuration);
        FadeAlphaGraphic(light1, 0.7f, fadeDuration);
        FadeAlphaGraphic(light2, 0.7f, fadeDuration);
        FadeVolumeWeight(1, fadeDuration);
        
        loadButton.interactable = true;
    }
    
    public void FadeAlphaGraphic(SkeletonGraphic graphic,float targetAlpha, float duration)
    {
        if (graphic != null)
        {
            Color sgColor = graphic.color;
            DOTween.To(() => sgColor.a,
                x => {
                    sgColor.a = x;
                    graphic.color = sgColor;
                },
                targetAlpha, duration).SetEase(Ease.InOutSine); 
        }
    }
    
    public void FadeAlphaSprite(Image sprite, float targetAlpha, float duration)
    {
        if (sprite != null)
        {
            Color srColor = sprite.color;
            DOTween.To(() => srColor.a,
                x => {
                    srColor.a = x;
                    sprite.color = srColor;
                },
                targetAlpha, duration).SetEase(Ease.InOutSine); 
        }
    }

    private void FadeVolumeWeight(float targetVolumeWeight, float duration)
    {
        if (volume != null)
        {
            DOTween.To(() => volume.weight,
                x => volume.weight = x,
                targetVolumeWeight,
                duration).SetEase(Ease.InOutSine); 
        }
    }
}
