using UnityEngine;

namespace PixelCrushers.DialogueSystem.SpineSupport
{

    /// <summary>
    /// This subclass of StandardUISubtitlePanel is aware of characters that
    /// have SpineDialogueActor components.
    /// </summary>
    public class SpineSubtitlePanel : StandardUISubtitlePanel
    {
        private SpineDialogueActor visibleSpineDialogueActor = null;

        public override void OpenOnStartConversation(Sprite portraitSprite, string portraitName, DialogueActor dialogueActor)
        {
            base.OpenOnStartConversation(portraitSprite, portraitName, dialogueActor);
            if (dialogueActor != null) ShowSpineDialogueActor(dialogueActor.transform);
        }

        public override void ShowSubtitle(Subtitle subtitle)
        {
            base.ShowSubtitle(subtitle);
            
            if (subtitle == null || subtitle.speakerInfo.Name == "내레이션" || string.IsNullOrEmpty(subtitle.speakerInfo.Name))
            {
                portraitName.gameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                portraitName.gameObject.transform.parent.gameObject.SetActive(true);
            }
            
            ShowSpineDialogueActor(subtitle.speakerInfo.transform);
        }

        public virtual void ShowSpineDialogueActor(Transform actorTransform)
        {
            if (actorTransform == null) return;
            var spineDialogueActor = actorTransform.GetComponent<SpineDialogueActor>();
            if (spineDialogueActor != visibleSpineDialogueActor)
            {
                if (visibleSpineDialogueActor != null) visibleSpineDialogueActor.Hide(this);
                if (spineDialogueActor != null) spineDialogueActor.Show(this);
                visibleSpineDialogueActor = spineDialogueActor;
            }
        }

        public override void Close()
        {
            if (visibleSpineDialogueActor != null) visibleSpineDialogueActor.Hide(this);
            visibleSpineDialogueActor = null;
            base.Close();
        }
    }
}
