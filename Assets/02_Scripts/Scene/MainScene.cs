using System;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    private void Awake()
    {
        Managers.Instance.ToString();
    }

    [SerializeField] private AudioClip backgroundMusic;
    private void Start()
    {
        Managers.SoundManager.Play(backgroundMusic, Sound.Bgm);
    }
}
