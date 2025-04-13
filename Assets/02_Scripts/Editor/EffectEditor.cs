using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Effect))]
public class EffectEditor : IdentifiedObjectEditor
{
    private SerializedProperty typeProperty;
    private SerializedProperty rarityProperty;
    private SerializedProperty isAllowDuplicateProperty;
    private SerializedProperty removeDuplicateTargetOptionProperty;

    private SerializedProperty isShowInUIProperty;

    private SerializedProperty isAllowLevelExceedDatasProperty;
    private SerializedProperty maxLevelProperty;
    private SerializedProperty effectDatasProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        typeProperty = serializedObject.FindProperty("type");
        rarityProperty = serializedObject.FindProperty("rarity");
        isAllowDuplicateProperty = serializedObject.FindProperty("isAllowDuplicate");
        removeDuplicateTargetOptionProperty = serializedObject.FindProperty("removeDuplicateTargetOption");

        isShowInUIProperty = serializedObject.FindProperty("isShowInUI");

        isAllowLevelExceedDatasProperty = serializedObject.FindProperty("isAllowLevelExceedDatas");

        maxLevelProperty = serializedObject.FindProperty("maxLevel");
        effectDatasProperty = serializedObject.FindProperty("effectDatas");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        // Lebel(=Inpectorâ�� ǥ�õǴ� ������ �̸�)�� ���̸� �ø�;
        float prevLevelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 175f;

        DrawSettings();
        DrawOptions();
        DrawEffectDatas();

        // Label�� ���̸� ������� �ǵ���
        EditorGUIUtility.labelWidth = prevLevelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSettings()
    {
        if (!DrawFoldoutTitle("Setting"))
            return;
        
        // Enum�� Toolbar ���·� �׷���
        CustomEditorUtility.DrawEnumToolbar(typeProperty);

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderline();
        EditorGUILayout.Space();
        CustomEditorUtility.DrawEnumToolbar(rarityProperty);
        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderline();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(isAllowDuplicateProperty);
        // �ߺ� ���� ��� Option�� true��� �ߺ� Effect�� ���� �ʿ䰡 �����Ƿ�
        // removeDuplicateTargetOption ������ �׸��� ����
        if (!isAllowDuplicateProperty.boolValue)
            CustomEditorUtility.DrawEnumToolbar(removeDuplicateTargetOptionProperty);
    }

    private void DrawOptions()
    {
        if (!DrawFoldoutTitle("Option"))
            return;

        EditorGUILayout.PropertyField(isShowInUIProperty);
    }

    private void DrawEffectDatas()
    {
        // Effect�� Data�� �ƹ��͵� �������� ������ 1���� �ڵ������� �������
        if (effectDatasProperty.arraySize == 0)
        {
            // �迭 ���̸� �÷��� ���ο� Element�� ����
            effectDatasProperty.arraySize++;
            // �߰��� Data�� Level�� 1�� ����
            effectDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
        }

        if (!DrawFoldoutTitle("Data"))
            return;

        EditorGUILayout.PropertyField(isAllowLevelExceedDatasProperty);

        // Level ���� ������ ���ٸ� MaxLevel�� �״�� �׷��ְ�,
        // ���� ������ �ִٸ� MaxLevel�� �������� ���� ��Ű�� �۾��� ��
        if (isAllowLevelExceedDatasProperty.boolValue)
            EditorGUILayout.PropertyField(maxLevelProperty);
        else
        {
            // Property�� �������� ���ϰ� GUI Enable�� false�� �ٲ�
            GUI.enabled = false;
            // ������ EffectData(= ���� ���� Level�� Data)�� ������
            var lastEffectData = effectDatasProperty.GetArrayElementAtIndex(effectDatasProperty.arraySize - 1);
            // maxLevel�� ������ Data�� Level�� ����
            maxLevelProperty.intValue = lastEffectData.FindPropertyRelative("level").intValue;
            // maxLevel Property�� �׷���
            EditorGUILayout.PropertyField(maxLevelProperty);
            GUI.enabled = true;
        }

        // effectDatas�� ���鼭 GUI�� �׷���
        for (int i = 0; i < effectDatasProperty.arraySize; i++)
        {
            var property = effectDatasProperty.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Data�� Level�� Data ������ ���� X Button�� �׷��ִ� Foldout Title�� �׷���
                // ��, ù��° Data(= index 0) ����� �ȵǱ� ������ X Button�� �׷����� ����
                // X Button�� ������ Data�� �������� true�� return��
                if (DrawRemovableLevelFoldout(effectDatasProperty, property, i, i != 0))
                {
                    // Data�� �����Ǿ����� �� �̻� GUI�� �׸��� �ʰ� �ٷ� ��������
                    // ���� Frame�� ó������ �ٽ� �׸��� ����
                    EditorGUILayout.EndVertical();
                    break;
                }

                if (property.isExpanded)
                {
                    // �鿩����
                    EditorGUI.indentLevel += 1;

                    var levelProperty = property.FindPropertyRelative("level");
                    // Level Property�� �׷��ָ鼭 Level ���� �����Ǹ� Level�� �������� EffectDatas�� ������������ ��������
                    DrawAutoSortLevelProperty(effectDatasProperty, levelProperty, i, i != 0);

                    var maxStackProperty = property.FindPropertyRelative("maxStack");
                    EditorGUILayout.PropertyField(maxStackProperty);
                    // maxStack�� �ּ� ���� 1 ���Ϸ� ������ ���ϰ� ��
                    maxStackProperty.intValue = Mathf.Max(maxStackProperty.intValue, 1);

                    var stackActionsProperty = property.FindPropertyRelative("stackActions");
                    var prevStackActionsSize = stackActionsProperty.arraySize;

                    EditorGUILayout.PropertyField(stackActionsProperty);

                    // stackActions�� Element�� �߰��ƴٸ�, ���� �߰��� Element�� Soft Copy�� action ������ Deep Copy ����
                    if (stackActionsProperty.arraySize > prevStackActionsSize)
                    {
                        // ������ �迭 Element(=���� ������� Element)�� ������
                        var lastStackActionProperty = stackActionsProperty.GetArrayElementAtIndex(prevStackActionsSize);
                        // Elememy���� action Property�� ã�ƿ�
                        var actionProperty = lastStackActionProperty.FindPropertyRelative("action");
                        // Deep Copy ����
                        CustomEditorUtility.DeepCopySerializeReference(actionProperty);
                    }

                    // StackAction���� stack ������ �Է� ������ �ִ� ���� MaxStack ������ ����
                    for (int stackActionIndex = 0; stackActionIndex < stackActionsProperty.arraySize; stackActionIndex++)
                    {
                        // Element�� ������
                        var stackActionProperty = stackActionsProperty.GetArrayElementAtIndex(stackActionIndex);
                        // Element���� stack Property�� ã�ƿ�
                        var stackProperty = stackActionProperty.FindPropertyRelative("stack");
                        // 1~MaxStack���� �� ����
                        stackProperty.intValue = Mathf.Clamp(stackProperty.intValue, 1, maxStackProperty.intValue);
                    }

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("action"));

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("runningFinishOption"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("duration"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCount"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("applyCycle"));

                    EditorGUILayout.PropertyField(property.FindPropertyRelative("customActions"));

                    // �鿩���� ����
                    EditorGUI.indentLevel -= 1;
                }
            }
            EditorGUILayout.EndVertical();
        }

        // EffectDatas�� ���ο� Data�� �߰��ϴ� Button
        if (GUILayout.Button("Add New Level"))
        {
            // �迭 ���̸� �÷��� ���ο� Element�� ����
            var lastArraySize = effectDatasProperty.arraySize++;
            // ���� Element Property�� ������
            var prevElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
            // �� Element Property�� ������
            var newElementProperty = effectDatasProperty.GetArrayElementAtIndex(lastArraySize);
            // �� Element�� Level�� ���� Element Level + 1
            var newElementLevel = prevElementProperty.FindPropertyRelative("level").intValue + 1;
            newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;

            // �� Element�� Soft Copy�� StackActions�� Action���� Deep Copy��
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("stackActions"), "action");

            // �� Element�� Soft Copy�� Action�� Deep Copy��
            CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("action"));

            // �� Element�� Soft Copy�� CustomAction�� Deep Copy��
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActions"));
        }
    }
}
