using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestReporterOnClick : MonoBehaviour
{
    [SerializeField]
    private Category category;
    [SerializeField]
    private TaskTarget target;
    [SerializeField]
    private int successCount;
    [SerializeField]
    private Button acceptButton;

    private void Start()
    {
        acceptButton.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        Report();
    }

    public void Report()
    {
        QuestSystem.Instance.ReceiveReport(category, target, successCount);
    }
}
