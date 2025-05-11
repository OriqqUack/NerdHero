using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ExitAlert : UiWindow
{
    [SerializeField] private Button claimButton;
    [SerializeField] private Button exitButton;

    protected override void Start()
    {
        base.Start();
        claimButton.onClick.AddListener(() => OnClickClaimBtn());
        exitButton.onClick.AddListener(() => Close());
    }

    private void OnClickClaimBtn()
    {
        Managers.SoundManager.Clear();
        Time.timeScale = 1;
        SceneTransitioner.Instance.StartTransitioning(SceneType.MainScene);
    }
}
