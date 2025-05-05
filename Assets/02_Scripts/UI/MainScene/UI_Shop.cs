using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : UiWindow
{
    [SerializeField] private Button closeBtn;

    protected override void Start()
    {
        base.Start();
        closeBtn.onClick.AddListener(() => CloseUI());
    }

    private void CloseUI()
    {
        UI_MainScene.Instance.CloseCurrentWindow();
        TabController.Instance.ResetTabs();
    }
}
