using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// EntityType�� StateMachine�� �����ϴ� Entity�� Type
public class StateMachine<EntityType>
{
    #region Event
    // State�� ���̵Ǿ����� �˸��� Event
    public delegate void StateChangedHandler(StateMachine<EntityType> stateMachine,
        State<EntityType> newState,
        State<EntityType> prevState,
        int layer);
    #endregion

    private class StateData
    {
        // State�� ����Ǵ� Layer
        public int Layer { get; private set; }
        // State�� ��� ����
        public int Priority { get; private set; }
        // Data�� ���� State
        public State<EntityType> State { get; private set; }
        // ���� State�� �ٸ� State�� �̾��� Transitions
        public List<StateTransition<EntityType>> Transitions { get; private set; } = new();

        public StateData(int layer, int priority, State<EntityType> state)
            => (Layer, Priority, State) = (layer, priority, state);
    }

    // Layer�� ������ �ִ� StateDatas(=Layer Dictionary), Dictionary�� Key�� Value�� StateData�� ���� State�� Type
    // ��, State�� Type�� ���� �ش� State�� ���� StateData�� ã�ƿ� �� ����
    private readonly Dictionary<int, Dictionary<Type, StateData>> stateDatasByLayer = new();
    // Layer�� Any Tansitions(���Ǹ� �����ϸ� �������� ToState�� ���̵Ǵ� Transition)
    private readonly Dictionary<int, List<StateTransition<EntityType>>> anyTransitionsByLayer = new();

    // Layer�� ���� �������� StateData(=���� �������� State)
    private readonly Dictionary<int, StateData> currentStateDatasByLayer = new();

    // StatMachine�� �����ϴ� Layer��, Layer�� �ߺ����� �ʾƾ��ϰ�, �ڵ� ������ ���ؼ� SortedSet�� �����
    private readonly SortedSet<int> layers = new();

    // StateMachine�� ������
    public EntityType Owner { get; private set; }

    public event StateChangedHandler onStateChanged;

    public void Setup(EntityType owner)
    {
        Debug.Assert(owner != null, $"StateMachine<{typeof(EntityType).Name}>::Setup - owner�� null�� �� �� �����ϴ�.");

        Owner = owner;

        AddStates();
        MakeTransitions();
        SetupLayers();
    }

    // Layer���� Current State�� �������ִ� ���ִ� �Լ�
    public void SetupLayers()
    {
        foreach ((int layer, var statDatasByType) in stateDatasByLayer)
        {
            // State�� �����ų Layer�� �������
            currentStateDatasByLayer[layer] = null;

            // �켱 ������ ���� ���� StateData�� ã�ƿ�
            var firstStateData = statDatasByType.Values.First(x => x.Priority == 0);
            // ã�ƿ� StateData�� State�� ���� Layer�� Current State�� ��������
            ChangeState(firstStateData);
        }
    }

    // ���� �������� CurrentStateData�� �����ϴ� �Լ�
    private void ChangeState(StateData newStateData)
    {
        // Layer�� �´� ���� �������� CurrentStateData�� ������
        var prevState = currentStateDatasByLayer[newStateData.Layer];

        prevState?.State.Exit();
        // ���� �������� CurrentStateData�� ���ڷ� ���� newStateData�� ��ü����
        currentStateDatasByLayer[newStateData.Layer] = newStateData;
        newStateData.State.Enter();

        // State�� ���̵Ǿ����� �˸�
        onStateChanged?.Invoke(this, newStateData.State, prevState.State, newStateData.Layer);
    }

    // newState�� Type�� �̿��� StateData�� ã�ƿͼ� ���� �������� CurrentStateData�� �����ϴ� �Լ�
    private void ChangeState(State<EntityType> newState, int layer)
    {
        // Layer�� ����� StateDatas�� newState�� ���� StateData�� ã�ƿ�
        var newStateData = stateDatasByLayer[layer][newState.GetType()];
        ChangeState(newStateData);
    }

    // Transition�� ������ Ȯ���Ͽ� ���̸� �õ��ϴ� �Լ�
    private bool TryTransition(IReadOnlyList<StateTransition<EntityType>> transtions, int layer)
    {
        foreach (var transition in transtions)
        {
            // Command�� �����Ѵٸ�, Command�� �޾��� ���� ���� �õ��� �ؾ������� �Ѿ
            // Command�� �������� �ʾƵ�, ���� ������ �������� ���ϸ� �Ѿ
            if (transition.TransitionCommand != StateTransition<EntityType>.kNullCommand || !transition.IsTransferable)
                continue;

            // CanTrainsitionToSelf(�ڱ� �ڽ����� ���� ���� �ɼ�)�� false�� �����ؾ��� ToState�� CurrentState�� ���ٸ� �Ѿ
            if (!transition.CanTrainsitionToSelf && currentStateDatasByLayer[layer].State == transition.ToState)
                continue;

            // ��� ������ �����Ѵٸ� ToState�� ����
            ChangeState(transition.ToState, layer);
            return true;
        }
        return false;
    }

    public void Update()
    {
        foreach (var layer in layers)
        {
            // Layer���� �������� ���� StateData�� ������
            var currentStateData = currentStateDatasByLayer[layer];

            // Layer�� ���� AnyTransitions�� ã�ƿ�
            bool hasAnyTransitions = anyTransitionsByLayer.TryGetValue(layer, out var anyTransitions);

            // AnyTansition�� �����ϸ�ٸ� AnyTransition���� ToState ���̸� �õ��ϰ�,
            // ������ ���� �ʾ� �������� �ʾҴٸ�, ���� StateData�� Transition�� �̿��� ���̸� �õ���
            if ((hasAnyTransitions && TryTransition(anyTransitions, layer)) ||
                TryTransition(currentStateData.Transitions, layer))
                continue;

            // �������� ���ߴٸ� ���� State�� Update�� ������
            currentStateData.State.Update();
        }
    }

    // Generic�� ���� StateMachine�� State�� �߰��ϴ� �Լ�
    // T�� State<EntityType> class�� ��ӹ��� Type�̿�����
    public void AddState<T>(int layer = 0) where T : State<EntityType>
    {
        // Layer �߰�, Set�̹Ƿ� �̹� Layer�� �����Ѵٸ� �߰����� ����
        layers.Add(layer);

        // Type�� ���� State�� ����
        var newState = Activator.CreateInstance<T>();
        newState.Setup(this, Owner, layer);

        // ���� stateDatasByLayer�� �߰����� ���� Layer��� Layer�� ��������
        if (!stateDatasByLayer.ContainsKey(layer))
        {
            // Layer�� StateData ����� Dictionary<Type, StateData> ����
            stateDatasByLayer[layer] = new();
            // Layer�� AnyTransitions ����� List<StateTransition<EntityType>> ����
            anyTransitionsByLayer[layer] = new();
        }

        Debug.Assert(!stateDatasByLayer[layer].ContainsKey(typeof(T)),
            $"StateMachine::AddState<{typeof(T).Name}> - �̹� ���°� �����մϴ�.");

        var stateDatasByType = stateDatasByLayer[layer];
        // StateData�� ���� Layer�� �߰�
        stateDatasByType[typeof(T)] = new StateData(layer, stateDatasByType.Count, newState);
    }

    // Transition�� �����ϴ� �Լ�
    // FromStateType�� ���� State�� Type
    // ToStateType�� ������ State�� Type
    // �� Tpye ��� State<EntityType> class�� �ڽ��̿�����
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
    {
        var stateDatas = stateDatasByLayer[layer];
        // StateDatas���� FromStateType�� State�� ���� StateData�� ã�ƿ�
        var fromStateData = stateDatas[typeof(FromStateType)];
        // StateDatas���� ToStateType�� State�� ���� StateData�� ã�ƿ�
        var toStateData = stateDatas[typeof(ToStateType)];
         
        // ���ڿ� ã�ƿ� Data�� ������ Transition�� ����
        // AnyTransition�� �ƴ� �Ϲ� Transition�� canTransitionToSelf ���ڰ� ������ true
        var newTransition = new StateTransition<EntityType>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition, true);
        // ������ Transition�� FromStateData�� Transition���� �߰�
        fromStateData.Transitions.Add(newTransition);
    }

    // MakeTransition �Լ��� Enum Command ����
    // Enum������ ���� Command�� Int�� ��ȯ�Ͽ� ���� �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer);
    
    // MakeTransition �Լ��� Command ���ڰ� ���� ����
    // NullCommand�� �־ �ֻ���� MakeTransition �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer);

    // MakeTransition �Լ��� Condition ���ڰ� ���� ����
    // Condition���� null�� �־ �ֻ���� MakeTransition �Լ��� ȣ���� 
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    // �� �Լ��� Enum ����(Command ���ڰ� Enum���̰� Condition ���ڰ� ����)
    // ���� ���ǵ� Enum���� MakeTransition �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    // AnyTransition�� ����� �Լ�
    // ToStateType�� ������ State�� Type, State<EntityType> class�� ����� Type�̿�����
    public void MakeAnyTransition<ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
    {
        var stateDatasByType = stateDatasByLayer[layer];
        // StateDatas���� ToStateType�� State�� ���� StateData�� ã�ƿ�
        var state = stateDatasByType[typeof(ToStateType)].State;
        // Transition ����, �������� ���Ǹ� ������ ������ ���̹Ƿ� FromState�� �������� ����
        var newTransition = new StateTransition<EntityType>(null, state, transitionCommand, transitionCondition, canTransitonToSelf);
        // Layer�� AnyTransition���� �߰�
        anyTransitionsByLayer[layer].Add(newTransition);
    }

    // MakeAnyTransition �Լ��� Enum Command ����
    // Enum������ ���� Command�� Int�� ��ȯ�Ͽ� ���� �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer, canTransitonToSelf);

    // MakeAnyTransition �Լ��� Command ���ڰ� ���� ����
    // NullCommand�� �־ �ֻ���� MakeTransition �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Func<State<EntityType>, bool> transitionCondition,
        int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer, canTransitonToSelf);

    // MakeAnyTransiiton�� Condition ���ڰ� ���� ����
    // Condition���� null�� �־ �ֻ���� MakeTransition �Լ��� ȣ���� 
    public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitonToSelf = false)
    where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);

    // �� �Լ��� Enum ����(Command ���ڰ� Enum���̰� Condition ���ڰ� ����)
    // ���� ���ǵ� Enum���� MakeAnyTransition �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);

    // Command�� �޾Ƽ� Transition�� �����ϴ� �Լ�
    public bool ExecuteCommand(int transitionCommand, int layer)
    {
        // AnyTransition���� Command�� ��ġ�ϰ�, ���� ������ �����ϴ� Transiton�� ã�ƿ�
        var transition = anyTransitionsByLayer[layer].Find(x =>
        x.TransitionCommand == transitionCommand && x.IsTransferable);

        // AnyTransition���� Transtion�� �� ã�ƿԴٸ� ���� �������� CurrentStateData�� Transitions����
        // Command�� ��ġ�ϰ�, ���� ������ �����ϴ� Transition�� ã�ƿ�
        transition ??= currentStateDatasByLayer[layer].Transitions.Find(x =>
        x.TransitionCommand == transitionCommand && x.IsTransferable);

        // ������ Transtion�� ã�ƿ��� ���ߴٸ� ���� ������ ����
        if (transition == null)
            return false;

        // ������ Transiton�� ã�ƿԴٸ� �ش� Transition�� ToState�� ����
        ChangeState(transition.ToState, layer);
        return true;
    }

    // ExecuteCommand�� Enum Command ����
    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => ExecuteCommand(Convert.ToInt32(transitionCommand), layer);

    // ��� Layer�� ������� ExecuteCommand �Լ��� �����ϴ� �Լ�
    // �ϳ��� Layer�� ���̿� �����ϸ� true�� ��ȯ 
    public bool ExecuteCommand(int transitionCommand)
    {
        bool isSuccess = false;

        foreach (int layer in layers)
        {
            if (ExecuteCommand(transitionCommand, layer))
                isSuccess = true;
        }

        return isSuccess;
    }

    // �� ExecuteCommand �Լ��� Enum Command ����
    public bool ExecuteCommand(Enum transitionCommand)
        => ExecuteCommand(Convert.ToInt32(transitionCommand));

    // ���� �������� CurrentStateData�� Message�� ������ �Լ�
    public bool SendMessage(int message, int layer, object extraData = null)
        => currentStateDatasByLayer[layer].State.OnReceiveMessage(message, extraData);

    // SendMessage �Լ��� Enum Message ����
    public bool SendMessage(Enum message, int layer, object extraData = null)
        => SendMessage(Convert.ToInt32(message), layer, extraData);

    // ��� Layer�� ���� �������� CurrentStateData�� ������� SendMessage �Լ��� �����ϴ� �Լ�
    // �ϳ��� CurrentStateData�� ������ Message�� �����ߴٸ� true�� ��ȯ
    public bool SendMessage(int message, object extraData = null)
    {
        bool isSuccess = false;
        foreach (int layer in layers)
        {
            if (SendMessage(message, layer, extraData))
                isSuccess = true;
        }
        return isSuccess;
    }

    // �� SendMessage �Լ��� Enum Message ����
    public bool SendMessage(Enum message, object extraData = null)
        => SendMessage(Convert.ToInt32(message), extraData);

    // ��� Layer�� ���� �������� CurrentState�� Ȯ���Ͽ�, ���� State�� T Type�� State���� Ȯ���ϴ� �Լ�
    // CurrentState�� T Type�ΰ� Ȯ�εǸ� ��� true�� ��ȯ��
    public bool IsInState<T>() where T : State<EntityType>
    {
        foreach ((_, StateData data) in currentStateDatasByLayer)
        {
            if (data.State.GetType() == typeof(T))
                return true;
        }
        return false;
    }

    // Ư�� Layer�� ������� �������� CurrentState�� T Type���� Ȯ���ϴ� �Լ�
    public bool IsInState<T>(int layer) where T : State<EntityType>
        => currentStateDatasByLayer[layer].State.GetType() == typeof(T);

    // Layer�� ���� �������� State�� ������
    public State<EntityType> GetCurrentState(int layer = 0) => currentStateDatasByLayer[layer].State;

    // Layer�� ���� �������� State�� Type�� ������
    public Type GetCurrentStateType(int layer = 0) => GetCurrentState(layer).GetType();

    // �ڽ� class���� ������ State �߰� �Լ�
    // �� �Լ����� AddState �Լ��� ����� State�� �߰����ָ��
    protected virtual void AddStates() { }
    // �ڽ� class���� ������ Transition ���� �Լ�
    // �� �Լ����� MakeTransition �Լ��� ����� Transition�� ������ָ� ��
    protected virtual void MakeTransitions() { }
}