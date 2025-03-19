using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField]
    private Quest[] quests;
    
    private List<Quest> currentQuests = new();
    private void Start()
    {
        foreach (var quest in quests)
        {
            if (quest.IsAcceptable && !QuestSystem.Instance.ContainsInCompleteQuests(quest))
            {
                //var newQuest = QuestSystem.Instance.Register(quest, DialogueManager.Instance.currentConversant);
                var newQuest = QuestSystem.Instance.Register(quest);
                newQuest.onWaitingForCompletion -= QuestWaitingCompleted;
                newQuest.onWaitingForCompletion += QuestWaitingCompleted;
                
                currentQuests.Add(newQuest);
                QuestVariableManager.EnsureQuestVariables(quest.CodeName);
            }
        } 
    }

    public void QuestComplete()
    {
        foreach (var quest in currentQuests)
        {
            if (quest.IsComplatable && !QuestSystem.Instance.ContainsInCompleteQuests(quest))
            {
                quest.Complete();
                QuestVariableManager.UpdateVariable($"{quest.CodeName}_Status", "Completed");
            }
        } 
    }

    private void QuestWaitingCompleted(Quest quest)
    {
        Debug.unityLogger.Log($"Quest {quest.CodeName} waiting for completion");
        Transform questOwner = QuestSystem.Instance.GetQuestOwner(quest);
        QuestVariableManager.UpdateVariable($"{quest.CodeName}_Status", "WaitingForCompletion");
    }
}
