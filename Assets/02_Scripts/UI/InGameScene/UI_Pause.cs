using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Pause : UiWindow
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button resumeButton;

    protected override void Start()
    {
        base.Start();
        exitButton.onClick.AddListener(() => UI_InGameScene.Instance.OpenExitAlert());
        resumeButton.onClick.AddListener(() => Close());
    }

    public override void Close()
    {
        base.Close();
        Time.timeScale = 1;
    }
}
