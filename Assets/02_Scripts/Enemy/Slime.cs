using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;
using AnimationState = Spine.AnimationState;

public class Slime : MonoBehaviour
{
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string onStartName;
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string onEndName;

    private Entity entity;
    private NavMeshAgent navMeshAgent;
    private EntityMovement entityMovement;

    public AnimationCurve speedCurve; // Inspector에서 조정 가능

    private float animationProgress = 0f; // 현재 애니메이션 진행률
    private bool isMoving = false; // 이동 중 여부
    private AnimationState animationState;
    private Stat moveSpeedStat;
    private void Awake()
    {
        entity = GetComponent<Entity>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        entityMovement = GetComponent<EntityMovement>();
    }

    private void Start()
    {
        moveSpeedStat = entity.Stats.GetStat("MOVE_SPEED");
        animationState = entity.Animator.skeletonAnimation.AnimationState;
        animationState.Event += HandleAnimationStateEvent;
    }

    private void Update()
    {
        if (!isMoving || navMeshAgent.isStopped) return;

        // 현재 재생 중인 애니메이션 가져오기
        TrackEntry currentTrack = animationState.GetCurrent(0);
        if (currentTrack != null && currentTrack.Animation.Name == "move")
        {
            // 애니메이션 진행률 (0~1)
            animationProgress = currentTrack.AnimationTime / currentTrack.AnimationEnd;
            animationProgress = Mathf.Clamp01(animationProgress); // 0~1로 제한

            // 애니메이션 진행률에 따라 속도 조절
            float speedMultiplier = speedCurve.Evaluate(animationProgress);
            navMeshAgent.speed = moveSpeedStat.Value * speedMultiplier; // 기본 속도 * 곡선 값
        }
    }

    private void HandleAnimationStateEvent(TrackEntry trackentry, Spine.Event e)
    {
        if (trackentry.Animation.Name != "move") return;

        if (e.Data.Name == onStartName)
        {
            navMeshAgent.isStopped = false;
            isMoving = true;
        }
        else if (e.Data.Name == onEndName)
        {
            navMeshAgent.isStopped = true;
            isMoving = false;
        }
    }
}
