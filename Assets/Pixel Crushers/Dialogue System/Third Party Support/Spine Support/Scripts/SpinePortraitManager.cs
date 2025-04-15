using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.SpineSupport
{

    /// <summary>
    /// Add to the Dialogue Manager to be able to use SpineActor() sequencer commands.
    /// </summary>
    public class SpinePortraitManager : MonoBehaviour
    {

        private static SpinePortraitManager m_instance = null;
        public static SpinePortraitManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<SpinePortraitManager>() ?? DialogueManager.instance.gameObject.AddComponent<SpinePortraitManager>();
                }
                return m_instance;
            }
        }

        private Dictionary<SpineDialogueActor, int> actors = new Dictionary<SpineDialogueActor, int>();
        private SpineDialogueActor focusedActor = null;

        private void OnEnable()
        {
            DialogueManager.instance.conversationEnded += OnConversationEnded;
        }

        private void OnDisable()
        {
            DialogueManager.instance.conversationEnded -= OnConversationEnded;
        }

        public void ShowSpineActor(SpineDialogueActor spineDialogueActor, int panelIndex)
        {            
            if (spineDialogueActor == null) return;
            if (actors.ContainsKey(spineDialogueActor))
            {
                if (actors[spineDialogueActor] != panelIndex)
                {
                    MoveSpineActorToPanel(spineDialogueActor, panelIndex);
                }
            }
            else
            {
                actors.Add(spineDialogueActor, panelIndex);
                MoveSpineActorToPanel(spineDialogueActor, panelIndex);
                SetTrigger(spineDialogueActor, spineDialogueActor.showTrigger);
            }
        }

        public void HideSpineActor(SpineDialogueActor spineDialogueActor)
        {
            if (spineDialogueActor == null) return;
            if (spineDialogueActor == focusedActor) focusedActor = null;
            SetTrigger(spineDialogueActor, spineDialogueActor.hideTrigger, false);
        }

        private void OnConversationLine(Subtitle subtitle)
        {
            if (string.IsNullOrEmpty(subtitle.formattedText.text)) return;
            StartCoroutine(CheckActorAtEndOfFrame(subtitle));
        }

        private IEnumerator CheckActorAtEndOfFrame(Subtitle subtitle)
        {
            yield return new WaitForEndOfFrame();
            var spineDialogueActor = subtitle.speakerInfo.transform.GetComponent<SpineDialogueActor>();
            if (focusedActor == null || focusedActor != spineDialogueActor)
            {
                if (focusedActor != null)
                {
                    SetTrigger(focusedActor, focusedActor.unfocusTrigger);
                }
                focusedActor = spineDialogueActor;
                if (spineDialogueActor != null)
                {
                    SetTrigger(spineDialogueActor, spineDialogueActor.focusTrigger);
                }
            }
        }

        private void OnConversationEnded(Transform conversationActor)
        {
            foreach (var spineDialogueActor in actors.Keys)
            {
                SetTrigger(spineDialogueActor, spineDialogueActor.hideTrigger, false);
            }
            actors.Clear();
            focusedActor = null;
        }

        private void MoveSpineActorToPanel(SpineDialogueActor spineDialogueActor, int panelIndex)
        {
            if (spineDialogueActor == null) return;
            if (spineDialogueActor.spineGameObject == null) return;
            var rt = spineDialogueActor.spineGameObject.GetComponent<RectTransform>();
            if (rt == null) return;
            var panel = GetPanel(panelIndex);
            if (panel == null) return;
            if (panel.panelState == UIPanel.PanelState.Closed) panel.Open();
            var panelRT = panel.GetComponent<RectTransform>();
            rt.pivot = panelRT.pivot;
            rt.anchoredPosition = panelRT.anchoredPosition;
            rt.anchorMax = panelRT.anchorMax;
            rt.anchorMin = panelRT.anchorMin;
            rt.sizeDelta = panelRT.sizeDelta;
            actors[spineDialogueActor] = panelIndex;
        }

        private StandardUISubtitlePanel GetPanel(int panelIndex)
        {
            var ui = DialogueManager.dialogueUI as StandardDialogueUI;
            if (ui == null) return null;
            var numPanels = ui.conversationUIElements.subtitlePanels.Length;
            if (!(0 <= panelIndex && panelIndex < numPanels)) return null;
            return ui.conversationUIElements.subtitlePanels[panelIndex];
        }

        private void SetTrigger(SpineDialogueActor spineDialogueActor, string triggerName, bool canvasState = true)
        {
            if (spineDialogueActor == null) return;
            if (spineDialogueActor.spineGameObject == null) return;
            var canvas = spineDialogueActor.spineGameObject.GetComponentInParent<Canvas>();
            if (canvas == null) return;
            canvas.enabled = canvasState;
            var animator = canvas.GetComponent<Animator>();
            if (animator == null) return;
            animator.SetTrigger(triggerName);
        }
    }
}
