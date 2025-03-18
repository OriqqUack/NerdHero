using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum Sound
{
    Bgm,
    Effect,
    MaxCount
}
public class SoundManager : MonoSingleton<SoundManager>
{
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    [HideInInspector] public AudioSource BgmSource;
    [HideInInspector] public AudioSource EffectSource;

    private void Awake()
    {
        string[] soundNames = System.Enum.GetNames(typeof(Sound)); // "Bgm", "Effect"
        for (int i = 0; i < soundNames.Length - 1; i++)
        {
            GameObject go = new GameObject { name = soundNames[i] };
            go.AddComponent<AudioSource>();
            if (soundNames[i] == "Bgm")
            {
                BgmSource = go.GetComponent<AudioSource>();
            }
            else
            {
                EffectSource = go.GetComponent<AudioSource>();
            }
            go.transform.parent = transform;
        }

        BgmSource.loop = true; // bgm 재생기는 무한 반복 재생
        //오브젝트화 필수
        DontDestroyOnLoad(this);
    }

    public void Play(string path, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Sound.Bgm) // BGM 배경음악 재생
        {
            if (BgmSource.isPlaying)
                BgmSource.Stop();

            BgmSource.pitch = pitch;
            BgmSource.clip = audioClip;
            BgmSource.Play();
        }
        else // Effect 효과음 재생
        {
            EffectSource.pitch = pitch;
            EffectSource.PlayOneShot(audioClip);
        }
    }

    private AudioClip GetOrAddAudioClip(string path, Sound type = Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}"; // Sound 폴더 안에 저장될 수 있도록

        AudioClip audioClip = null;

        if (type == Sound.Bgm) // BGM 배경음악 클립 붙이기
        {
            audioClip = Resources.Load<AudioClip>(path);
        }
        else // Effect 효과음 클립 붙이기
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }

    public void Clear()
    {
        BgmSource.Stop();
        EffectSource.Stop();
        // 효과음 Dictionary 비우기
        _audioClips.Clear();
    }
}
