using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// EntityType�� State�� �����ϴ� Entity�� Type
// StateMachine�� EntityType�� ��ġ�ؾ���
public abstract class State<EntityType>
{
    public StateMachine<EntityType> Owner { get; private set; }
    public EntityType Entity { get; private set; }
    // State�� StateMachine�� ��ϵ� Layer ��ȣ
    public int Layer { get; private set; }

    // StatMachine���� ����� Setup�Լ�
    public void Setup(StateMachine<EntityType> owner, EntityType entity, int layer)
    {
        Owner = owner;
        Entity = entity;
        Layer = layer;

        Setup();
    }

    // Awake ��Ȱ�� ���� Setup �Լ�
    protected virtual void Setup() { }

    // State�� ���۵� �� ����� �Լ�
    public virtual void Enter() { }
    // State�̰� �������� �� �� �����Ӹ��� ����Ǵ� �Լ�
    public virtual void Update() { }
    // State�� ���� �� ����� �Լ�
    public virtual void Exit() { }
    // StateMachine�� ���� �ܺο��� Message�� �Ѿ���� �� ó���ϴ� �Լ�
    // Message��°� State���� Ư�� �۾��� �϶�� �����ϱ� ���� �����ڰ� ���� ��ȣ
    public virtual bool OnReceiveMessage(int message, object data) => false;

}