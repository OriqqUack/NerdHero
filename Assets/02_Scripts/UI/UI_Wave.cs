using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Wave : MonoBehaviour
{
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float waitingTime = 3.0f;

    private Animator animator;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        WaveManager.Instance.OnWaveStart += OpenUI;
    }

    private void OpenUI(int waveIndex)
    {
        text.text = string.Format("WAVE {00}", waveIndex);
        
        animator.SetTrigger("Open");
    }
}
