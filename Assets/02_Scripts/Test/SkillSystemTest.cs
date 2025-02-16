using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystemTest : MonoBehaviour
{
    [SerializeField]
    private Skill testSkill;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Test");
            Test();
        }
#endif
    }

    [ContextMenu("Test")]
    public void Test() => StartCoroutine("TestCoroutine");

    private IEnumerator TestCoroutine()
    {
        Debug.Log($"<color=yellow>[SkillSystemTest] Start</color>");

        var skillSystem = GetComponent<SkillSystem>();
        if (!skillSystem.Register(testSkill))
            Debug.LogAssertion($"{testSkill.CodeName}�� ������� ���߽��ϴ�.");

        Debug.Log($"Skill ��� ����: {testSkill.CodeName}");

        var skill = skillSystem.Find(testSkill);
        Debug.Assert(skill != null, $"{skill.CodeName}�� ã�� ���߽��ϴ�.");

        Debug.Log("testSkill�� ���� skillSystem�� ��ϵ� Skill�� �˻��� ����.");
        Debug.Log($"{skill.CodeName} ��� �õ�...");

        if (!skillSystem.Use(skill))
            Debug.LogAssertion($"{skill.CodeName}�� ������� ���߽��ϴ�.");

        Debug.Log("Skill�� Unregister�ϰ� Test�� �����Ϸ��� S Key�� ��������.");

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (!skillSystem.Unregister(skill))
                    Debug.LogAssertion($"{skill.CodeName}�� ��� �������� ���߽��ϴ�.");
                break;
            }
            yield return null;
        }

        Debug.Log($"Skill ���� ����");
        Debug.Log($"<color=green>[SkillSystemTest] Success</color>");
    }
}
