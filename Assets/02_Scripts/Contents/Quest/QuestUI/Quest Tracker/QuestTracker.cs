using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using DG.Tweening; // DOTween 네임스페이스 추가

public class QuestTracker : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI questTitleText;
    [SerializeField]
    private TaskDescriptor taskDescriptorPrefab;

    [Header("Quest Completion Notifier")]
    [SerializeField]
    private bool isActiveNotifier = false;
    [SerializeField]
    private string titleDescription;
    [SerializeField]
    private float showTime = 3f;

    private Queue<Quest> reservedQuests = new Queue<Quest>();
    private StringBuilder stringBuilder = new StringBuilder();

    private Dictionary<Task, TaskDescriptor> taskDesriptorsByTask = new Dictionary<Task, TaskDescriptor>();

    private Quest targetQuest;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 offScreenPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        offScreenPosition = new Vector2(Screen.width + 500, originalPosition.y); // 화면 밖 오른쪽
    }

    private void OnDestroy()
    {
        if (targetQuest != null)
        {
            targetQuest.onNewTaskGroup -= UpdateTaskDescriptos;
            targetQuest.onCompleted -= DestroySelf;
        }

        foreach (var tuple in taskDesriptorsByTask)
        {
            var task = tuple.Key;
            task.onSuccessChanged -= UpdateText;
        }
    }

    public void Setup(Quest targetQuest, Color titleColor)
    {
        this.targetQuest = targetQuest;

        questTitleText.text = targetQuest.Category == null ?
            targetQuest.DisplayName :
            $"[{targetQuest.Category.DisplayName}] {targetQuest.DisplayName}";

        questTitleText.color = titleColor;

        targetQuest.onNewTaskGroup += UpdateTaskDescriptos;
        targetQuest.onCompleted += DestroySelf;

        var taskGroups = targetQuest.TaskGroups;
        UpdateTaskDescriptos(targetQuest, taskGroups[0]);

        if (taskGroups[0] != targetQuest.CurrentTaskGroup)
        {
            for (int i = 1; i < taskGroups.Count; i++)
            {
                var taskGroup = taskGroups[i];
                UpdateTaskDescriptos(targetQuest, taskGroup, taskGroups[i - 1]);

                if (taskGroup == targetQuest.CurrentTaskGroup)
                    break;
            }
        }

        if (isActiveNotifier)
        {
            rectTransform.anchoredPosition = offScreenPosition;
            rectTransform.DOAnchorPos(originalPosition, 1f).SetEase(Ease.OutCubic);
        }
    }

    private void UpdateTaskDescriptos(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        foreach (var task in currentTaskGroup.Tasks)
        {
            var taskDesriptor = Instantiate(taskDescriptorPrefab, transform);
            taskDesriptor.UpdateText(task);
            task.onSuccessChanged += UpdateText;

            taskDesriptorsByTask.Add(task, taskDesriptor);
        }

        if (prevTaskGroup != null)
        {
            foreach (var task in prevTaskGroup.Tasks)
            {
                var taskDesriptor = taskDesriptorsByTask[task];
                taskDesriptor.UpdateTextUsingStrikeThrough(task);
            }
        }
    }

    private void UpdateText(Task task, int currentSucess, int prevSuccess)
    {
        taskDesriptorsByTask[task].UpdateText(task);
    }

    private void DestroySelf(Quest quest)
    {
        if (isActiveNotifier)
            Notify(quest);
        else
            Destroy(gameObject);
    }

    private void Notify(Quest quest)
    {
        reservedQuests.Enqueue(quest);
        StartCoroutine(ShowNotice());
    }

    private IEnumerator ShowNotice()
    {
        var waitSeconds = new WaitForSeconds(showTime);

        while (reservedQuests.TryDequeue(out Quest quest))
        {
            questTitleText.text = titleDescription.Replace("%{dn}", quest.DisplayName);
            foreach (var reward in quest.Rewards)
            {
                stringBuilder.Append(reward.Description);
                stringBuilder.Append(" ");
                stringBuilder.Append(reward.Quantity);
                stringBuilder.Append(" ");
            }

            // 인 효과
            rectTransform.anchoredPosition = offScreenPosition;
            rectTransform.DOAnchorPos(originalPosition, 1f).SetEase(Ease.OutCubic);

            yield return waitSeconds;

            // 아웃 효과
            rectTransform.DOAnchorPos(offScreenPosition, 1f).SetEase(Ease.InCubic);
            yield return new WaitForSeconds(0.6f); // 애니메이션 마무리 시간 대기

            stringBuilder.Clear();
        }

        Destroy(gameObject);
    }
}
