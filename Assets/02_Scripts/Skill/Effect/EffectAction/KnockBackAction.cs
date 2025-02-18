using System.Collections;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class KnockBackAction : EffectAction
{
    [SerializeField]
    private Category removeTargetCategory;

    public override void Start(Effect effect, Entity user, Entity target, int level, float scale)
    {
        Stat stat = effect.User.Stats.GetStat("KNOCKBACK_FORCE");
        float knockBackValue = stat.Value;
        float knockBackDuration = effect.Duration;

        Vector3 knockBackDir = (target.transform.position - user.transform.position).normalized;
        knockBackDir.y = 0; // 수직 이동 방지

        Rigidbody targetRb = target.Rigidbody;
        if (targetRb != null)
        {
            targetRb.linearVelocity = Vector3.zero; // 기존 속도 초기화
            target.StartCoroutine(ApplyKnockBack(targetRb, knockBackDir, knockBackValue, knockBackDuration));
        }
    }

    private IEnumerator ApplyKnockBack(Rigidbody rb, Vector3 direction, float force, float duration)
    {
        float elapsed = 0f;
        float initialForce = force;
    
        rb.AddForce(direction * initialForce, ForceMode.Impulse); // 즉시 넉백 적용

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 부드러운 감속 (시간이 지날수록 힘을 점차 줄임)
            float t = elapsed / duration;
            rb.linearVelocity = direction * (initialForce * (1 - t)); // 점점 속도를 감소

            yield return null;
        }

        // 넉백이 끝나면 속도를 완전히 0으로 설정
        rb.linearVelocity = Vector3.zero;
    }

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        target.SkillSystem.RemoveEffectAll(removeTargetCategory);
        target.StateMachine.ExecuteCommand(EntityStateCommand.ToKnockBackState);
        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
        => target.StateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);

    public override object Clone() => new KnockBackAction() { removeTargetCategory = removeTargetCategory };

}
