using UnityEngine;

namespace PixelCrushers.DialogueSystem.SpineSupport
{

    /// <summary>
    /// This addon for the DialogueActor component handles a Spine character avatar.
    /// If a SpineSubtitlePanel finds a SpineDialogueActor, it uses its Show and Hide methods.
    /// </summary>
    [RequireComponent(typeof(DialogueActor))]
    public class SpineDialogueActor : MonoBehaviour
    {
        public GameObject spineGameObject;

        public string showTrigger = "Show";
        public string hideTrigger = "Hide";
        public string focusTrigger = "Focus";
        public string unfocusTrigger = "Unfocus";

        protected bool wasInactive = false;
        private UIAnimatorMonitor m_animatorMonitor = null;
        public UIAnimatorMonitor animatorMonitor
        {
            get
            {
                if (m_animatorMonitor == null && spineGameObject != null)
                {
                    m_animatorMonitor = new UIAnimatorMonitor(gameObject);
                }
                return m_animatorMonitor;
            }
        }


        public virtual void Show(StandardUISubtitlePanel subtitlePanel)
        {
            if (spineGameObject == null) return;
            wasInactive = !spineGameObject.activeSelf;
            spineGameObject.SetActive(true);
            animatorMonitor.SetTrigger(showTrigger, null, false);
        }

        public virtual void Hide(StandardUISubtitlePanel subtitlePanel)
        {
            if (spineGameObject == null) return;
            animatorMonitor.SetTrigger(hideTrigger, OnHidden, true);
        }

        protected void OnHidden()
        {
            if (wasInactive) spineGameObject.SetActive(false);
        }
    }
}