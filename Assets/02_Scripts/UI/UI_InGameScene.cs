using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_InGameScene : MonoSingleton<UI_InGameScene>
{
    [SerializeField] private WindowHolder pause;
    [SerializeField] private WindowHolder exitAlert;
    [SerializeField] private WindowHolder revive;
    [SerializeField] private WindowHolder gameEnd;
    [SerializeField] private WindowHolder cardSelec;

    private UiWindow _cardSelectUI;
    private bool _isGameEnd;
    private Queue<Action> _cardQueue = new Queue<Action>();
    private Coroutine _waitCoroutine;
    private void Start()
    {
        DOTween.defaultTimeScaleIndependent = true;
        WaveManager.Instance.OnWaveEnd += OpenGameEnd;
        WaveManager.Instance.PlayerEntity.Stats.GetStat("LEVEL").onValueChanged += OpenCardSelec;
        WaveManager.Instance.PlayerEntity.Stats.ClearedStats();
        WaveManager.Instance.OnWaveEnd += Clear;
    }
    
    public UiWindow GetCardSelectUI() => _cardSelectUI;

    public void OpenPause()
    {
        Time.timeScale = 0;
        pause.OpenWindow();
    }
    public void OpenExitAlert() => exitAlert.OpenWindow();
    public void OpenRevive()
    {
        Time.timeScale = 0;
        revive.OpenWindow();
    }

    public void OpenGameEnd()
    {
        _isGameEnd = true;
        Time.timeScale = 0;
        gameEnd.OpenWindow();
    }

    private void OpenCardSelec(Stat stat, float currentValue, float prevValue)
    {
        if (_isGameEnd) return;
    
        Time.timeScale = 0;

        // 요청을 큐에 저장
        _cardQueue.Enqueue(() =>
        {
            _cardSelectUI = cardSelec.OpenWindow();
        });

        // 만약 대기 코루틴이 없으면 시작
        if (_waitCoroutine == null)
        {
            _waitCoroutine = StartCoroutine(HandleCardQueue());
        }
    }

    private IEnumerator HandleCardQueue()
    {
        while (_cardQueue.Count > 0)
        {
            // 현재 카드가 살아있으면 기다려
            yield return new WaitUntil(() => !_cardSelectUI);

            // 큐에서 다음 카드 열기
            var openAction = _cardQueue.Dequeue();
            openAction.Invoke();
        }

        _waitCoroutine = null;
    }

    private void Clear()
    {
        WaveManager.Instance.PlayerEntity.Stats.GetStat("LEVEL").onValueChanged -= OpenCardSelec;
    }
}
