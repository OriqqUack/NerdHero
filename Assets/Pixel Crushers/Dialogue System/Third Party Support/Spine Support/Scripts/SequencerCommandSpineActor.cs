using UnityEngine;
using PixelCrushers.DialogueSystem.SpineSupport;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Syntax: SpineActor(actor, panel#|hide)
    /// </summary>
    public class SequencerCommandSpineActor : SequencerCommand
    {
        private void Awake()
        {
            var actorName = GetParameter(0);
            var hide = string.Equals("hide", GetParameter(1), System.StringComparison.OrdinalIgnoreCase);
            int panelIndex = hide ? -1 : GetParameterAsInt(1);
            var spineDialogueActor = CharacterInfo.GetRegisteredActorTransform(actorName).GetComponent<SpineDialogueActor>();
            if (spineDialogueActor == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: SpineActor(" + GetParameters() + "): Can't find SpineDialogueActor.");
            }
            else
            {
                if (DialogueDebug.logInfo) Debug.Log("Dialogue System: Sequencer: SpineActor(" + spineDialogueActor + ", " + (hide ? "hide" : panelIndex.ToString()) + ")", spineDialogueActor);
                if (hide)
                {
                    SpinePortraitManager.instance.HideSpineActor(spineDialogueActor);
                }
                else
                {
                    SpinePortraitManager.instance.ShowSpineActor(spineDialogueActor, panelIndex);
                }
            }
            Stop();
        }
    }

}
