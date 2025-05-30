using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    /*private Entity entity;

    private void Start()
    {
        entity = GetComponent<Entity>();
        entity.SkillSystem.onSkillTargetSelectionCompleted += ReserveSkill;


        MouseController.Instance.onLeftClicked += SelectTarget;
    }

    private void OnEnable()
        => MouseController.Instance.onRightClicked += MoveToPosition;

    private void OnDisable()
        => MouseController.Instance.onRightClicked -= MoveToPosition;
    private void OnDestroy()
        => MouseController.Instance.onLeftClicked -= SelectTarget;

    private void SelectTarget(Vector2 mousePosition)
    {
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
        {
            var entity = hitInfo.transform.GetComponent<Entity>();
            if (entity)
                EntityHUD.Instance.Show(entity);
        }
    }

    private void MoveToPosition(Vector2 mousePosition)
    {
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            entity.Movement.Destination = hitInfo.point;
            entity.SkillSystem.CancelReservedSkill();
        }
    }

    private void ReserveSkill(SkillSystem skillSystem, Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result)
    {
        // �˻� ����� OutOfRange�� �ƴϰų� Skill�� ���� SearchingTargetState�� �ƴ϶�� return���� ��������
        // Target Select�� Skill�� ������ ���� ���� ������ �Ͼ �� �ִµ�,
        // Skill�� SearchingTargetState�� ���� ������ �õ���
        if (result.resultMessage != SearchResultMessage.OutOfRange ||
            !skill.IsInState<SearchingTargetState>())
            return;

        entity.SkillSystem.ReserveSkill(skill);

        if (result.selectedTarget)
            entity.Movement.TraceTarget = result.selectedTarget.transform;
        else
            entity.Movement.Destination = result.selectedPosition;
    }*/
}
