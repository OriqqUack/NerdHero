using System.Diagnostics;

public class EntityStateMachine : MonoStateMachine<Entity>
{
    protected override void AddStates()
    {
        AddState<EntityDefaultState>();
        AddState<DeadState>();
        AddState<RollingState>();
        // Skill�� Casting ���� �� Entity�� ����
        AddState<CastingSkillState>();
        // Skill�� Charging ���� �� Entity�� ����
        AddState<ChargingSkillState>();
        // Skill�� Preceding Action ���� �� Entity�� ����
        AddState<InSkillPrecedingActionState>();
        // Skill�� �ߵ� ���� �� Entity�� ����
        AddState<InSkillActionState>();
        // Entity�� Stun CC�⸦ �¾��� ���� ����
        AddState<StunningState>();
        AddState<KnockBackState>();
        // Entity�� Sleep CC�⸦ �¾��� ���� ����
        AddState<SleepingState>();
    }

    protected override void MakeTransitions()
    {
        // Default State
        //MakeTransition<EntityDefaultState, RollingState>(state => Owner.Movement?.IsRolling ?? false);
        MakeTransition<EntityDefaultState, CastingSkillState>(EntityStateCommand.ToCastingSkillState);
        MakeTransition<EntityDefaultState, ChargingSkillState>(EntityStateCommand.ToChargingSkillState);
        MakeTransition<EntityDefaultState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<EntityDefaultState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);

        // Rolling State
        //MakeTransition<RollingState, EntityDefaultState>(state => !Owner.Movement.IsRolling);

        // Skill State
            // Casting State
        MakeTransition<CastingSkillState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<CastingSkillState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<CastingSkillState, EntityDefaultState>(state => !IsSkillInState<CastingState>(state));

            // Charging State
        MakeTransition<ChargingSkillState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<ChargingSkillState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<ChargingSkillState, EntityDefaultState>(state => !IsSkillInState<ChargingState>(state));

            // PrecedingAction State
        MakeTransition<InSkillPrecedingActionState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<InSkillPrecedingActionState, EntityDefaultState>(state => !IsSkillInState<InPrecedingActionState>(state));

            //Action State
        MakeTransition<InSkillActionState, EntityDefaultState>(state => (state as InSkillActionState).IsStateEnded);

        // CC State
            // Stuning State
        MakeAnyTransition<StunningState>(EntityStateCommand.ToStunningState);
        MakeAnyTransition<KnockBackState>(EntityStateCommand.ToKnockBackState);
            // Sleeping State
        MakeAnyTransition<SleepingState>(EntityStateCommand.ToSleepingState);

        MakeAnyTransition<EntityDefaultState>(EntityStateCommand.ToDefaultState);

        MakeAnyTransition<DeadState>(state => Owner.IsDead);
        MakeTransition<DeadState, EntityDefaultState>(state => !Owner.IsDead);
    }

    private bool IsSkillInState<T>(State<Entity> state) where T : State<Skill>
        => (state as EntitySkillState).RunningSkill.IsInState<T>();
}
