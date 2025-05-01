using PixelCrushers.DialogueSystem;
using UnityEngine;

public class TutorialEventTrigger : MonoSingleton<TutorialEventTrigger>
{
    [SerializeField] private Transform actor;
    [SerializeField] private string[] conversationList;
    public bool Seen = false;
    private int index = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (Seen) return;
        EntityHUD hud = other.GetComponent<EntityHUD>();
        if (hud)
        {
            DialogueManager.StartConversation(conversationList[index], actor);
            Seen = true;
        }
    }
}
