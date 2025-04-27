using PixelCrushers.DialogueSystem;
using UnityEngine;

public class TutorialEventTrigger : MonoBehaviour
{
    [SerializeField] private Transform actor;
    private bool _seen = false;
    private void OnTriggerEnter(Collider other)
    {
        if (_seen) return;
        EntityHUD hud = other.GetComponent<EntityHUD>();
        if (hud)
        {
            DialogueManager.StartConversation("AttackTutorial", actor);
            Camera.main.GetComponent<CameraFollow>().ZoomIn(3f);
            _seen = true;
            this.enabled = false;
        }
    }
}
