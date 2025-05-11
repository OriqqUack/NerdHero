using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Revive : UiWindow
{
    [SerializeField] private TextMeshProUGUI reviveCount;
    [SerializeField] private Button reviveButton;

    protected override void Start()
    {
        base.Start();
        
        //reviveButton.onClick.AddListener();
        //StartCoroutine(Counting());
        reviveButton.onClick.AddListener(() => Revive());
    }

    private void Revive()
    {
        (WaveManager.Instance.PlayerTransform.GetComponent<Entity>().Movement as PlayerMovement)?.ReviveEvent();
        Close();
    }

    public override void Close()
    {
        base.Close();
        Time.timeScale = 1;
    }

    public void CloseImmediately()
    {
        if(_closeCallback != null) _closeCallback(_windowHolder);
        //SoundManager.Instance.Play(closeSound);
        Destroy(gameObject);
        Time.timeScale = 1;
        UI_InGameScene.Instance.OpenGameEnd();
    }

    public void SceneTransition()
    {
        SceneTransitioner.Instance.StartTransitioning(SceneType.MainScene);
    }

    /*IEnumerator Counting()
    {
        int count = 5;
        while (count > 0)
        {
            reviveCount.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }
    }*/
}
