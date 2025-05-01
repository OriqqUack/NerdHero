using System;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class OnSkillButtonClicked : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => ClickedButton());
    }

    private void ClickedButton()
    {
        DialogueManager.StopConversation();
        SpotlightController.Instance.OffSpotlight();
        Time.timeScale = 1f;
    }
}
