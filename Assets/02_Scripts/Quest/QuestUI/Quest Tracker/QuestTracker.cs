using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class QuestTracker : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI questTitleText;
    [SerializeField]
    private TaskDescriptor taskDescriptorPrefab;

    [UnderlineTitle("Quest Completion Notifier")] [SerializeField]
    private bool isActiveNotifier = false;
    [SerializeField]
    private string titleDescription;
    [SerializeField]
    private float showTime = 3f;
    
    private Queue<Quest> reservedQuests = new Queue<Quest>();
    private StringBuilder stringBuilder = new StringBuilder();

    private Dictionary<Task, TaskDescriptor> taskDesriptorsByTask = new Dictionary<Task, TaskDescriptor>();

    private Quest targetQuest;
    private Animator animator;

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
        if(isActiveNotifier)
            animator = GetComponent<Animator>();
        
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
        
        if(isActiveNotifier)
            animator.SetTrigger("Show");
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
        if(isActiveNotifier)
            Notify(quest);
        else
            Destroy(gameObject);
    }
    
    private void Notify(Quest quest)
    {
        reservedQuests.Enqueue(quest);

        StartCoroutine("ShowNotice");
    }
    
    private IEnumerator ShowNotice()
    {
        var waitSeconds = new WaitForSeconds(showTime);

        if(isActiveNotifier)
            animator.SetTrigger("Complete");
        
        Quest quest;
        while (reservedQuests.TryDequeue(out quest))
        {
            questTitleText.text = titleDescription.Replace("%{dn}", quest.DisplayName);
            foreach (var reward in quest.Rewards)
            {
                stringBuilder.Append(reward.Description);
                stringBuilder.Append(" ");
                stringBuilder.Append(reward.Quantity);
                stringBuilder.Append(" ");
            }
            stringBuilder.Clear();

            yield return waitSeconds;
        }
        
        if(isActiveNotifier)
            animator.SetTrigger("Hide");

        Destroy(gameObject, 0.5f);
    }
}
