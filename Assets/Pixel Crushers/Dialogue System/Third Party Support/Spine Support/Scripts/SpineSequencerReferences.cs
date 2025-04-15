using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.SpineSupport
{

    /// <summary>
    /// This component holds references to Spine AnimationReferenceAssets
    /// so the SpineAnimation() sequencer command can access them.
    /// </summary>
    public class SpineSequencerReferences : MonoBehaviour
    {
        [HelpBox("Assign a SkeletonAnimation or SkeletonGraphic. Then assign animations that the SpineAnimation() sequencer command can use.", HelpBoxMessageType.None)]
        public Spine.Unity.SkeletonAnimation skeletonAnimation;
        public Spine.Unity.SkeletonGraphic skeletonGraphic;
        public List<Spine.Unity.AnimationReferenceAsset> animationReferenceAssets = new List<Spine.Unity.AnimationReferenceAsset>();
    }
}