using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Skill))]
public class SkillEditor : IdentifiedObjectEditor
{
    private SerializedProperty typeProperty;
    private SerializedProperty useTypeProperty;
    private SerializedProperty movingTypeProperty;
    
    private SerializedProperty executionTypeProperty;
    private SerializedProperty applyTypeProperty;
    private SerializedProperty needSelectionResultTypeProperty;
    private SerializedProperty targetSelectionTimingOptionProperty;
    private SerializedProperty targetSearchTimingOptionProperty;

    private SerializedProperty acquisitionConditionsProperty;
    private SerializedProperty acquisitionCostsProperty;

    private SerializedProperty useConditionsProperty;

    private SerializedProperty isAllowLevelExceedDatasProperty;
    private SerializedProperty maxLevelProperty;
    private SerializedProperty defaultLevelProperty;
    private SerializedProperty skillDatasProperty;

    // Toolbar Button���� �̸�
    private readonly string[] customActionsToolbarList = new[] { "Cast", "Charge", "Preceding", "Action" };
    // Skill Data���� ������ Toolbar Button�� Index ��
    private Dictionary<int, int> customActionToolbarIndexesByLevel = new();

    private bool IsPassive => typeProperty.enumValueIndex == (int)SkillType.Passive;
    private bool IsToggleType => useTypeProperty.enumValueIndex == (int)SkillUseType.Toggle;
    // Toggle, Passive Type�� ���� ������� �ʴ� �������� �������� ���� ����
    private bool IsDrawPropertyAll => !IsToggleType && !IsPassive;

    protected override void OnEnable()
    {
        base.OnEnable();

        typeProperty = serializedObject.FindProperty("type");
        useTypeProperty = serializedObject.FindProperty("useType");
        movingTypeProperty = serializedObject.FindProperty("movingType");
        
        executionTypeProperty = serializedObject.FindProperty("executionType");
        applyTypeProperty = serializedObject.FindProperty("applyType");
        needSelectionResultTypeProperty = serializedObject.FindProperty("needSelectionResultType");

        targetSelectionTimingOptionProperty = serializedObject.FindProperty("targetSelectionTimingOption");
        targetSearchTimingOptionProperty = serializedObject.FindProperty("targetSearchTimingOption");

        acquisitionConditionsProperty = serializedObject.FindProperty("acquisitionConditions");
        acquisitionCostsProperty = serializedObject.FindProperty("acquisitionCosts");

        useConditionsProperty = serializedObject.FindProperty("useConditions");

        isAllowLevelExceedDatasProperty = serializedObject.FindProperty("isAllowLevelExceedDatas");
        maxLevelProperty = serializedObject.FindProperty("maxLevel");
        defaultLevelProperty = serializedObject.FindProperty("defaultLevel");
        skillDatasProperty = serializedObject.FindProperty("skillDatas");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        float prevLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 220f;

        DrawSettings();
        DrawAcquisition();
        DrawUseConditions();
        DrawSkillDatas();

        EditorGUIUtility.labelWidth = prevLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSettings()
    {
        if (!DrawFoldoutTitle("Setting"))
            return;

        CustomEditorUtility.DrawEnumToolbar(typeProperty);
        if (!IsPassive)
            CustomEditorUtility.DrawEnumToolbar(useTypeProperty);
        else
            // instant�� ����
            useTypeProperty.enumValueIndex = 0;
        CustomEditorUtility.DrawEnumToolbar(movingTypeProperty);
        
        if (IsDrawPropertyAll)
        {
            EditorGUILayout.Space();
            CustomEditorUtility.DrawUnderline();
            EditorGUILayout.Space();

            CustomEditorUtility.DrawEnumToolbar(executionTypeProperty);
            CustomEditorUtility.DrawEnumToolbar(applyTypeProperty);
        }
        else
        {
            // auto�� ����
            executionTypeProperty.enumValueIndex = 0;
            // instant�� ����
            applyTypeProperty.enumValueIndex = 0;
        }

        EditorGUILayout.Space();
        CustomEditorUtility.DrawUnderline();
        EditorGUILayout.Space();

        CustomEditorUtility.DrawEnumToolbar(needSelectionResultTypeProperty);
        CustomEditorUtility.DrawEnumToolbar(targetSelectionTimingOptionProperty);
        CustomEditorUtility.DrawEnumToolbar(targetSearchTimingOptionProperty);
    }

    private void DrawAcquisition()
    {
        if (!DrawFoldoutTitle("Acquisition"))
            return;

        EditorGUILayout.PropertyField(acquisitionConditionsProperty);
        EditorGUILayout.PropertyField(acquisitionCostsProperty);
    }

    private void DrawUseConditions()
    {
        if (!DrawFoldoutTitle("Use Condition"))
            return;

        EditorGUILayout.PropertyField(useConditionsProperty);
    }

    private void DrawSkillDatas()
    {
        // Skill�� Data�� �ƹ��͵� �������� ������ 1���� �ڵ������� �������
        if (skillDatasProperty.arraySize == 0)
        {
            // �迭 ���̸� �÷��� ���ο� Element�� ����
            skillDatasProperty.arraySize++;
            // �߰��� Data�� Level�� 1�� ����
            skillDatasProperty.GetArrayElementAtIndex(0).FindPropertyRelative("level").intValue = 1;
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
            var lastIndex = skillDatasProperty.arraySize - 1;
            // ������ SkillData(= ���� ���� Level�� Data)�� ������
            var lastSkillData = skillDatasProperty.GetArrayElementAtIndex(lastIndex);
            // maxLevel�� ������ Data�� Level�� ����
            maxLevelProperty.intValue = lastSkillData.FindPropertyRelative("level").intValue;
            // maxLevel Property�� �׷���
            EditorGUILayout.PropertyField(maxLevelProperty);
            GUI.enabled = true;
        }

        EditorGUILayout.PropertyField(defaultLevelProperty);

        for (int i = 0; i < skillDatasProperty.arraySize; i++)
        {
            var property = skillDatasProperty.GetArrayElementAtIndex(i);

            var isUseCastProperty = property.FindPropertyRelative("isUseCast");
            var isUseChargeProperty = property.FindPropertyRelative("isUseCharge");
            var chargeDurationProperty = property.FindPropertyRelative("chargeDuration");
            var chargeTimeProperty = property.FindPropertyRelative("chargeTime");
            var needChargeTimeToUseProperty = property.FindPropertyRelative("needChargeTimeToUse");

            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Data�� Level�� Data ������ ���� X Button�� �׷��ִ� Foldout Title�� �׷���
                // ��, ù��° Data(= index 0) ����� �ȵǱ� ������ X Button�� �׷����� ����
                // X Button�� ������ Data�� �������� true�� return��
                if (DrawRemovableLevelFoldout(skillDatasProperty, property, i, i != 0))
                {
                    // Data�� �����Ǿ����� �� �̻� GUI�� �׸��� �ʰ� �ٷ� ��������
                    // ���� Frame�� ó������ �ٽ� �׸��� ����
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUI.indentLevel += 1;

                if (property.isExpanded)
                {
                    // SkillData Property ���η� �� -> Property == level field;
                    property.NextVisible(true);

                    DrawAutoSortLevelProperty(skillDatasProperty, property, i, i != 0);

                    // Level Up
                    for (int j = 0; j < 2; j++)
                    {
                        property.NextVisible(false);
                        EditorGUILayout.PropertyField(property);
                    }

                    // PrecedingAction
                    // Toggle Type�� ���� PrecedingAction�� ������� ���� ���̹Ƿ�,
                    // Instant Type�� ���� PrecedingAction ������ ������
                    property.NextVisible(false);
                    if (useTypeProperty.enumValueIndex == (int)SkillUseType.Instant)
                        EditorGUILayout.PropertyField(property);

                    // Action And Setting
                    for (int j = 0; j < 8; j++)
                    {
                        // ���� ������ Property�� �̵��ϸ鼭 �׷���
                        property.NextVisible(false);
                        EditorGUILayout.PropertyField(property);
                    }

                    // Cast
                    property.NextVisible(false);
                    if (IsDrawPropertyAll && !isUseChargeProperty.boolValue)
                        EditorGUILayout.PropertyField(property);
                    else
                        property.boolValue = false;

                    property.NextVisible(false);
                    if (isUseCastProperty.boolValue)
                        EditorGUILayout.PropertyField(property);

                    // Charge
                    property.NextVisible(false);
                    if (IsDrawPropertyAll && !isUseCastProperty.boolValue)
                        EditorGUILayout.PropertyField(property);

                    for (int j = 0; j < 5; j++)
                    {
                        property.NextVisible(false);
                        if (isUseChargeProperty.boolValue)
                            EditorGUILayout.PropertyField(property);
                    }

                    // �ִ� chargeTime ���� chargeDuration ������ ����
                    chargeTimeProperty.floatValue = Mathf.Min(chargeTimeProperty.floatValue, chargeDurationProperty.floatValue);

                    // �ִ� needChargeTime ���� chargeTime ������ ����
                    needChargeTimeToUseProperty.floatValue = Mathf.Min(chargeTimeProperty.floatValue, needChargeTimeToUseProperty.floatValue);

                    // Effect
                    property.NextVisible(false);
                    EditorGUILayout.PropertyField(property);

                    // EffectSelector�� level ������ effect�� �ִ� level ������
                    for (int j = 0; j < property.arraySize; j++)
                    {
                        var effectSelectorProperty = property.GetArrayElementAtIndex(j);
                        // Selector�� level Property�� ������
                        var levelProperty = effectSelectorProperty.FindPropertyRelative("level");
                        // Selector�� ���� effect�� ������
                        var effect = effectSelectorProperty.FindPropertyRelative("effect").objectReferenceValue as Effect;
                        var maxLevel = effect != null ? effect.MaxLevel : 0;
                        var minLevel = maxLevel == 0 ? 0 : 1;
                        levelProperty.intValue = Mathf.Clamp(levelProperty.intValue, minLevel, maxLevel);
                    }

                    // Animation
                    for (int j = 0; j < 5; j++)
                    {
                        property.NextVisible(false);
                        EditorGUILayout.PropertyField(property);
                    }

                    // Custom Action - UnderlineTitle
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Custom Action", EditorStyles.boldLabel);
                    CustomEditorUtility.DrawUnderline();

                    // Custom Action - Toolbar
                    // �ѹ��� ��� Array ������ �� �׸��� ���� �����ϴ� Toolbar�� ���� ������ Array�� ������ �� �ְ���.
                    var customActionToolbarIndex = customActionToolbarIndexesByLevel.ContainsKey(i) ? customActionToolbarIndexesByLevel[i] : 0;
                    // Toolbar�� �ڵ� �鿩����(EditorGUI.indentLevel)�� ������ �ʾƼ� ���� �鿩���⸦ ����
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(12);
                        customActionToolbarIndex = GUILayout.Toolbar(customActionToolbarIndex, customActionsToolbarList);
                        customActionToolbarIndexesByLevel[i] = customActionToolbarIndex;
                    }
                    GUILayout.EndHorizontal();

                    // Custom Action
                    for (int j = 0; j < 4; j++)
                    {
                        property.NextVisible(false);
                        if (j == customActionToolbarIndex)
                            EditorGUILayout.PropertyField(property);
                    }
                }
                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add New Level"))
        {
            // Level Change
            var lastArraySize = skillDatasProperty.arraySize++;
            var prevElementalProperty = skillDatasProperty.GetArrayElementAtIndex(lastArraySize - 1);
            var newElementProperty = skillDatasProperty.GetArrayElementAtIndex(lastArraySize);
            var newElementLevel = prevElementalProperty.FindPropertyRelative("level").intValue + 1;
            newElementProperty.FindPropertyRelative("level").intValue = newElementLevel;
            newElementProperty.isExpanded = true;

            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("levelUpConditions"));
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("levelUpCosts"));

            CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("precedingAction"));

            CustomEditorUtility.DeepCopySerializeReference(newElementProperty.FindPropertyRelative("action"));

            // Costs Deep Copy
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("costs"));

            // TargetSearcher SelectionAction Deep Copy
            CustomEditorUtility.DeepCopySerializeReference(newElementProperty
                .FindPropertyRelative("targetSearcher")
                .FindPropertyRelative("selectionAction"));

            // TargetSearcher SearchAction Deep Copy
            CustomEditorUtility.DeepCopySerializeReference(newElementProperty
               .FindPropertyRelative("targetSearcher")
               .FindPropertyRelative("searchAction"));

            //customActionsOnCast; customActionsOnCharge;
            //customActionsOnPrecedingAction; customActionsOnAction;
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActionsOnCast"));
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActionsOnCharge"));
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActionsOnPrecedingAction"));
            CustomEditorUtility.DeepCopySerializeReferenceArray(newElementProperty.FindPropertyRelative("customActionsOnAction"));
        }
    }
}