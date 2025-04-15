The default Spine integration uses SkeletonAnimations, which exist in world space.
This example shows how to use Spine SkeletonGraphics in Unity UI.

It uses two custom scripts:

SpineDialogueUI: A subclass of StandardDialogueUI that observes these custom conversation fields:
- "Panel # Actor" specifies the actor in panel #.
- "Panel # Start Visible" specifies whether panel # should start visible.

(Make sure the dialogue UI uses SpineDialogueUI and SpineSubtitlePanel.)

SpineDialogueActorUI: A subclass of SpineDialogueActor that handles SkeletonGraphics.

In the Dialogue Editor, give each actor a portrait image, even though it won't be used.
This is how the Dialogue System knows to show portrait info for the actor.

Add a SpineDialogueActor to each actor, and assign its SkeletonGraphic. During conversations,
the SkeletonGraphic will be moved to the appropriate subtitle panel.