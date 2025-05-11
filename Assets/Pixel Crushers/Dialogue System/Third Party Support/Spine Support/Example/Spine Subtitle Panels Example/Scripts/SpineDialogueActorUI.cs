using Spine.Unity;
using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.SpineSupport
{

    /// <summary>
    /// This addon for the DialogueActor component is a subclass of SpineDialogueActor
    /// for Spine SkeletonGraphics (UI elements). It puts the Spine character inside
    /// the subtitle panel's portrait image.
    /// </summary>
    [RequireComponent(typeof(DialogueActor))]
    public class SpineDialogueActorUI : SpineDialogueActor
    {
        [SerializeField] private AudioClip audioClip;
        public override void Show(StandardUISubtitlePanel subtitlePanel)
        {
            if (spineGameObject == null || subtitlePanel == null || subtitlePanel.portraitImage == null) return;
            wasInactive = !spineGameObject.activeSelf;

            // Deactivate any old SkeletonGraphics that were in this panel's portrait image area:
            foreach (Transform child in subtitlePanel.transform)
            {
                var otherSkeletonGraphic = child.GetComponent<SkeletonGraphic>();
                if (otherSkeletonGraphic != null) otherSkeletonGraphic.gameObject.SetActive(false);
            }

            // Put my SkeletonGraphic in the portrait image area:
            spineGameObject.SetActive(true);
            spineGameObject.transform.SetParent(subtitlePanel.transform, false);
            if (subtitlePanel.portraitName != null)
            { // Place behind portrait name:
                spineGameObject.transform.SetSiblingIndex(subtitlePanel.portraitName.gameObject.transform.GetSiblingIndex());
            }
            spineGameObject.GetComponent<RectTransform>().anchoredPosition = subtitlePanel.portraitImage.GetComponent<RectTransform>().anchoredPosition;
            subtitlePanel.GetComponent<Animator>().Rebind();
            subtitlePanel.GetComponent<Animator>().SetTrigger(subtitlePanel.showAnimationTrigger);
        }

        public void PlayAudio()
        {
            if(audioClip)
                Managers.SoundManager.Play(audioClip);
        }
    }
}
