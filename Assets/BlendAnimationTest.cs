using Spine;
using Spine.Unity;
using UnityEngine;

public class BlendAnimationTest : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;

    void Start()
    {
        var state = skeletonAnimation.AnimationState;
        
        // 0번 트랙: 기본 걷기 애니메이션 (하체 포함)
        state.SetAnimation(0, "walk", true);

        // 1번 트랙: 공격 애니메이션 (상체만 적용)
        TrackEntry attackEntry = state.SetAnimation(1, "attack", false);

        // 특정 Bone만 적용하기 위해 MixBlend 설정
        attackEntry.MixBlend = MixBlend.Add;
        attackEntry.MixAttachmentThreshold = 0.5f;
    }
}