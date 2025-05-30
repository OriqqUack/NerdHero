using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using Spine;
using Spine.Unity;
using AnimationState = Spine.AnimationState;

//Spine 애니메이션의 이벤트 트리거
public class AnimationMove : MonoBehaviour
{
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    [SerializeField] private string onStartName;
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    [SerializeField] private string onEndName;
    [SerializeField] private string animationName;
    [SerializeField] private AnimationCurve speedCurve; // Inspector에서 조정 가능

    private Entity _entity;
    private FollowerEntity _navMeshAgent;
    private EntityMovement _entityMovement;
    

    private float _animationProgress = 0f; // 현재 애니메이션 진행률
    private bool _isMoving = false; // 이동 중 여부
    private AnimationState _animationState;
    private Stat _moveSpeedStat;
    private void Awake()
    {
        _entity = GetComponent<Entity>();
        _navMeshAgent = GetComponent<FollowerEntity>();
        _entityMovement = GetComponent<EntityMovement>();
    }

    private void Start()
    {
        _moveSpeedStat = _entity.Stats.GetStat("MOVE_SPEED");
        _animationState = _entity.Animator.skeletonAnimation.AnimationState;
        _animationState.Event += HandleAnimationStateEvent;
    }

    private void Update()
    {
        if (!_isMoving || _navMeshAgent.isStopped) return;

        // 현재 재생 중인 애니메이션 가져오기
        TrackEntry currentTrack = _animationState.GetCurrent(0);
        if (currentTrack != null && currentTrack.Animation.Name == animationName)
        {
            // 애니메이션 진행률 (0~1)
            _animationProgress = currentTrack.AnimationTime / currentTrack.AnimationEnd;
            _animationProgress = Mathf.Clamp01(_animationProgress); // 0~1로 제한

            // 애니메이션 진행률에 따라 속도 조절
            float speedMultiplier = speedCurve.Evaluate(_animationProgress);
            _navMeshAgent.maxSpeed = _moveSpeedStat.Value * speedMultiplier; // 기본 속도 * 곡선 값
        }
    }

    private void HandleAnimationStateEvent(TrackEntry trackentry, Spine.Event e)
    {
        if (trackentry.Animation.Name != animationName) return;

        if (e.Data.Name == onStartName)
        {
            _navMeshAgent.isStopped = false;
            _isMoving = true;
        }
        else if (e.Data.Name == onEndName)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.velocity = Vector3.zero;
            _isMoving = false;
        }
    }
}
