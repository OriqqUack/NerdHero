using System;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField] private UserInfo userInfo;
    [SerializeField] private AudioClip backgroundMusic;

    private void Awake()
    {
        Managers.Instance.ToString();
        userInfo.GetUserInfoFromBackend();
    }

    private void Start()
    {
        Managers.SoundManager.Play(backgroundMusic, Sound.Bgm);
    }
}
