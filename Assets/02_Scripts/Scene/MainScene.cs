using System;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;
    private void Start()
    {
        SoundManager.Instance.Play(backgroundMusic, Sound.Bgm);
    }
}
