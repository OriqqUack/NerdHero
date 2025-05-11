using System;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class StoryScene : MonoBehaviour
{
    [SerializeField] private Transform actor;
    [SerializeField] private AudioClip bgSound;

    private void Start()
    {
        DialogueManager.StartConversation("스토리_도시상가_주변", actor);
    }

}
