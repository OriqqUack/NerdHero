using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;

public class IntroController : MonoBehaviour
{
    [SerializeField] private Image blackScreen;     // 검정 → 흰 배경 전환용
    [SerializeField] private GameObject logo;       // 로고 오브젝트
    [SerializeField] private SkeletonGraphic graphic;
    [SerializeField] private GoogleBackendAutoLoginManager loginUI;    // 로그인 UI
    [SerializeField] private AudioClip bgMusic;
    
    [SerializeField] private float logoAppearDuration = 1.2f;
    [SerializeField] private float whiteFadeDuration = 1.0f;
    [SerializeField] private float logoMoveDuration = 1.0f;
    
    void Start()
    {
        Sequence seq = DOTween.Sequence();

        //logoRect = logo.GetComponent<RectTransform>();

        blackScreen.color = Color.black;
        seq.AppendInterval(3f);

        /*logo.SetActive(true);
        logo.transform.localScale = Vector3.zero;


        seq.AppendInterval(0.1f);
        
        // 1. 로고 '뽀잉' 등장 + 흔들림
        seq.Append(logoRect.DOScale(1.2f, logoAppearDuration * 0.6f).SetEase(Ease.OutBack));
        seq.Join(logoRect.DOShakeRotation(logoAppearDuration * 0.6f, new Vector3(0, 0, 10), 5, 90)); // Z축 흔들림

        // 2. 로고가 살짝 줄어들며 안착
        seq.Append(logoRect.DOScale(1.0f, logoAppearDuration * 0.4f).SetEase(Ease.InOutSine));

        // 3. 살짝 기다렸다가 화면 밝아지기
        seq.AppendInterval(0.2f);*/
        
        seq.Append(blackScreen.DOFade(0f, whiteFadeDuration));
        seq.Join(graphic.DOFade(0f, whiteFadeDuration));
        seq.AppendCallback(() => Managers.SoundManager.Play(bgMusic, Sound.Bgm));

        /*// 4. 로고가 지정 위치 + 회전 + 스케일로 튕기듯 이동
        seq.Append(logoRect.DOAnchorPos(targetTransform.GetComponent<RectTransform>().anchoredPosition, logoMoveDuration).SetEase(Ease.InOutBack));
        seq.Join(logoRect.DORotateQuaternion(targetTransform.rotation, logoMoveDuration).SetEase(Ease.InOutBack));
        seq.Join(logo.transform.DOScale(targetTransform.localScale, logoMoveDuration).SetEase(Ease.InOutBack));*/

        // 5. 로그인 UI 등장
        seq.AppendCallback(() =>
        {
            blackScreen.raycastTarget = false;
            loginUI.Init();
        });
    }

}