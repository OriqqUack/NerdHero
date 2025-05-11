using System;
using System.Collections.Generic;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class TutorialScene : MonoBehaviour
{
    [SerializeField] private Transform actor;
    [SerializeField] private AudioClip bgSound;
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
        GameManager.Instance.CurrentIslandIndex = -1;
        WaveManager.Instance.OnWaveEnd += () => { DialogueManager.StartConversation("Tutorial_End", actor); };
        WaveManager.Instance.OnWaveEnd += () => { Managers.BackendManager.UpdateField("isClearedTutorial", true); };
        
        _playerStats.Owner.BaseAttack.gameObject.SetActive(false);
        
        if(bgSound)
            Managers.SoundManager.Play(bgSound, Sound.Bgm);
        
        _playerStats.Owner.onKill += MonsterKill;
    }

    private void MonsterKill(Entity entity)
    {
        DialogueManager.StartConversation("Tutorial_MonsterKill", actor);
        _playerStats.Owner.onKill -= MonsterKill;
    }
    
    private void MonsterSpawn(List<Entity> entities)
    {
        if (WaveManager.Instance.CurrentWave == 1)
        {
            DOVirtual.DelayedCall(1.2f, () =>
            {
                DialogueManager.StartConversation("Tutorial_Move", actor);
                _playerStats.Owner.BaseAttack.gameObject.SetActive(true);
            });
        }
        if (WaveManager.Instance.CurrentWave == 3)
        {
            DialogueManager.StartConversation("TutorialIndicator", actor);
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
