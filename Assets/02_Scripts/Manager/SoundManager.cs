using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public enum Sound
{
    Bgm,
    Effect,
    MaxCount
}
public class SoundManager
{
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    [HideInInspector] public AudioSource BgmSource;
    [HideInInspector] public AudioSource EffectSource;

    public void Init()
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
            go.transform.parent = Managers.Instance.transform;
        }

        BgmSource.loop = true; // bgm 재생기는 무한 반복 재생
        //오브젝트화 필수
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
            {
                FadeOutBgm(1f, () =>
                {
                    FadeInBgm(audioClip, 1f, 1f);
                });
                BgmSource.Stop();
            }
            else
            {
                BgmSource.pitch = pitch;
                BgmSource.clip = audioClip;
                BgmSource.Play();
            }
        }
        else // Effect 효과음 재생
        {
            if (Mathf.Approximately(pitch, 1.0f))
            {
                EffectSource.PlayOneShot(audioClip);
            }
            else
            {
                // 오브젝트 이름 지정
                string objName = $"Effect_{audioClip.name}_{pitch:F2}";

                // 이미 존재하는 오브젝트 확인
                Transform existing = Managers.Instance.transform.Find(objName);

                AudioSource temp;

                if (existing != null)
                {
                    // 오디오소스 재활용
                    temp = existing.GetComponent<AudioSource>();
                }
                else
                {
                    // 새로 생성
                    GameObject go = new GameObject(objName);
                    go.transform.parent = Managers.Instance.transform;

                    temp = go.AddComponent<AudioSource>();
                    temp.clip = audioClip;
                    temp.pitch = pitch;
                    temp.volume = EffectSource.volume;
                    temp.outputAudioMixerGroup = EffectSource.outputAudioMixerGroup;

                    Object.Destroy(go, audioClip.length / pitch);
                }

                temp.Play();
            }
        }


    }

    public void FadeOutBgm(float duration = 1f, System.Action onComplete = null)
    {
        if (BgmSource.isPlaying)
        {
            DOTween.To(() => BgmSource.volume, x => BgmSource.volume = x, 0f, duration)
                .OnComplete(() =>
                {
                    BgmSource.Stop();
                    onComplete?.Invoke();
                });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void FadeInBgm(AudioClip newClip, float duration = 1f, float volume = 1f)
    {
        BgmSource.clip = newClip;
        BgmSource.volume = 0f;
        BgmSource.Play();
        DOTween.To(() => BgmSource.volume, x => BgmSource.volume = x, volume, duration);
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
