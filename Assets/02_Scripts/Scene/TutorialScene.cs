using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class TutorialScene : MonoBehaviour
{
    [SerializeField] private Transform actor;
    private Stats _playerStats;
    private void Start()
    {
        _playerStats = WaveManager.Instance.PlayerEntity.Stats;
        DialogueManager.instance.conversationStarted += OnDialogueStart;
        DialogueManager.instance.conversationEnded += OnDialogueEnd;
        _playerStats.GetStat("LEVEL").onValueChanged += OnLevelChanged;
        _playerStats.GetStat("ENERGY").onValueMax += OnStatMaxChanged;
        WaveManager.Instance.OnMonsterSpawn += MonsterSpawn;
        DialogueManager.StartConversation("TutorialStart", actor);
    }
    
    private void MonsterSpawn(List<Entity> entities)
    {
        if (WaveManager.Instance.CurrentWave == 2)
        {
            DialogueManager.StartConversation("TutorialIndicator", actor);
        }
        if (WaveManager.Instance.CurrentWave == 3)
        {
        }
    }

    private void OnLevelChanged(Stat stat, float currentLevel, float prevLevel)
    {
        DialogueManager.StartConversation("TutorialCardSelec", actor);
        WaveManager.Instance.PlayerEntity.Stats.GetStat("LEVEL").onValueChanged -= OnLevelChanged;
        CardAnimator.onCardSpawned += CardSpawned;
    }

    private void OnStatMaxChanged(Stat stat, float current, float prev)
    {
        DialogueManager.StartConversation("TutorialSkill", actor);
        WaveManager.Instance.PlayerEntity.BaseAttack.GetComponent<Collider>().enabled = false;
        WaveManager.Instance.PlayerEntity.Animator.PlayAnimationForState("idle", 0);
        WaveManager.Instance.PlayerEntity.Movement.enabled = false;
        _playerStats.GetStat("ENERGY").onValueMax -= OnStatMaxChanged;
    }

    private void CardSpawned()
    {
        GameObject go = (UI_InGameScene.Instance.GetCardSelectUI() as CardAnimator).Cards[0];
        SpotlightController.Instance.SetTarget(go.transform, null, false);
        CardAnimator.onCardSpawned -= CardSpawned;
        DialogueManager.instance.conversationEnded -= OnDialogueEnd;
        CardAnimator.onCardDelete += CardDestroyed;
    }

    private void CardDestroyed()
    {
        DialogueManager.instance.conversationEnded += OnDialogueEnd;
    }
    
    private void OnDialogueEnd(Transform actor)
    {
        Time.timeScale = 1;
        WaveManager.Instance.PlayerEntity.Movement.ReStart();
    }

    private void OnDialogueStart(Transform actor)
    {
        Time.timeScale = 0;
    }
}
