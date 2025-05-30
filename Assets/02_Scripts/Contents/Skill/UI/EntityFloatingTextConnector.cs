using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityFloatingTextConnector : MonoBehaviour
{
    [SerializeField] private Transform textSpawnPoint;
    [SerializeField] private Sprite alertSprite;
    private Entity entity;
    
    private void Start()
    {
        entity = GetComponent<Entity>();
        entity.onTakeDamage += OnTakeDamage;
        entity.StateMachine.onStateChanged += OnStateChanged;
        entity.Stats.HPStat.onValueChanged += OnHPValueChanged;

        if (entity.ControlType != EntityControlType.Player)
        {
            EntityMovement entityMovement =  entity.Movement as EntityMovement;
            entityMovement.OnFindTarget += OnInsight;
        }
    }

    private void OnTakeDamage(Entity entity, Entity instigator, object causer, float damage)
    {
        Effect effect = causer as Effect;

        float value = instigator.Stats.Damage.Value;
        bool isCritical = damage >= value * ( 1 + instigator.Stats.CriticalDamage.Value );
        if(isCritical)
        {
            FloatingTextView.Instance.ShowCritical(textSpawnPoint, $"-{Mathf.RoundToInt(damage)}");
            return;
        }
        
        if(effect)
            FloatingTextView.Instance.Show(textSpawnPoint, $"-{Mathf.RoundToInt(damage)}", Color.red, null, effect);
        else
            FloatingTextView.Instance.Show(textSpawnPoint, $"-{Mathf.RoundToInt(damage)}", Color.red);
    }

    private void OnStateChanged(StateMachine<Entity> stateMachine, State<Entity> newState, State<Entity> prevState, int layer)
    {
        var ccState = newState as EntityCCState;
        if (ccState == null)
            return;

        FloatingTextView.Instance.Show(textSpawnPoint, ccState.Description, Color.magenta);
    }

    private void OnHPValueChanged(Stat stat, float currentValue, float prevValue)
    {
        var value = currentValue - prevValue;
        if (value > 0)
            FloatingTextView.Instance.Show(textSpawnPoint, $"+{Mathf.RoundToInt(value)}", Color.green);
    }

    private void OnInsight(EntityMovement entityMovement)
    {
        FloatingTextView.Instance.Show(textSpawnPoint, null, Color.red, alertSprite);

    }
}
