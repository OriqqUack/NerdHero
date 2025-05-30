using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public static class CustomEditorUtility
{
    private readonly static GUIStyle titleStyle;

    static CustomEditorUtility()
    {
        // ����Ƽ ���ο� ���ǵǾ��ִ� ShurikenModuleTitle Style�� Base�� ��
        titleStyle = new GUIStyle("ShurikenModuleTitle")
        {
            // ����Ƽ Default Label�� font�� ������
            font = new GUIStyle(EditorStyles.label).font,
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            // title�� �׸� �����¿� ������ ������ ��
            border = new RectOffset(15, 7, 4, 4),
            // ���̴� 26
            fixedHeight = 26f,
            // ���� Text�� ��ġ�� ������
            contentOffset = new Vector2(20f, -2f)
        };
    }

    public static bool DrawFoldoutTitle(string title, bool isExpanded, float space = 15f)
    {
        // space��ŭ �� ���� ���
        EditorGUILayout.Space(space);

        // titleStyle�� ������ ������ Inspector�󿡼� �ǹٸ� ��ġ�� ������
        var rect = GUILayoutUtility.GetRect(16f, titleStyle.fixedHeight, titleStyle);
        // TitleStyle�� �����Ų Box�� �׷���
        GUI.Box(rect, title, titleStyle);

        // ���� Editor�� Event�� ������
        // Editor Event�� ���콺 �Է�, GUI ���� �׸���(Repaint), Ű���� �Է� �� Editor �󿡼� �Ͼ�� ����
        var currentEvent = Event.current;
        // Toggle Button�� ��ġ�� ũ�⸦ ����
        // ��ġ�� ��� �׸� �ڽ��� ��ǥ���� ��¦ ������ �Ʒ�, �� Button�� ��, ��� ������ �� ���°� ��.
        var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);

        // Event�� Repaint(=GUI�� �׸��� Ȥ�� �ٽ� �׸���)�� �ܼ��� foldout button�� ������
        if (currentEvent.type == EventType.Repaint)
            EditorStyles.foldout.Draw(toggleRect, false, false, isExpanded, false);
        // Event�� MouseDown�̰� mousePosition�� rect�� ����������(=Mouse Pointer�� ������ �׷��� Box�ȿ� ����) Click ����
        else if (currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
        {
            isExpanded = !isExpanded;
            // Use �Լ��� ������� ������ ���� Event�� ó������ ���� ������ �ǴܵǾ� ���� ��ġ�� �ִ� �ٸ� GUI�� ���� ���۵� �� ����.
            // event ó���� ������ �׻� Use�� ���� event�� ���� ó���� ������ Unity�� �˷��ִ°� ����
            currentEvent.Use();
        }

        return isExpanded;
    }

    // FoldoutTitle�� �׸��� ���ÿ� ���ڷ� ���� Dictionary�� Expand ��Ȳ�� ������� ����
    public static bool DrawFoldoutTitle(IDictionary<string, bool> isFoldoutExpandedesByTitle, string title, float space = 15f)
    {
        if (!isFoldoutExpandedesByTitle.ContainsKey(title))
            isFoldoutExpandedesByTitle[title] = true;

        isFoldoutExpandedesByTitle[title] = DrawFoldoutTitle(title, isFoldoutExpandedesByTitle[title], space);
        return isFoldoutExpandedesByTitle[title];
    }

    public static void DrawUnderline(float height = 1f)
    {
        // ���������� �׸� GUI�� ��ġ�� ũ�� ������ ���� Rect ����ü�� ������
        var lastRect = GUILayoutUtility.GetLastRect();
        // Rect ����ü�� indent(=�鿩����)�� ����� ������ ��ȯ��
        lastRect = EditorGUI.IndentedRect(lastRect);
        // rect�� y���� ���� GUI�� ���̸�ŭ ����(=��, y���� ���� GUI �ٷ� �Ʒ��� ��ġ�ϰ� ��)
        lastRect.y += lastRect.height;
        lastRect.height = height;
        // rect ���� �̿��ؼ� ������ ��ġ�� heightũ���� Box�� �׸�
        // height�� 1�̶�� ���� GUI �ٷ� �Ʒ��� ũ�Ⱑ 1�� Box, �� Line�� �׷����Ե�
        EditorGUI.DrawRect(lastRect, Color.gray);
    }

    public static void DrawEnumToolbar(SerializedProperty enumProperty)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(enumProperty.displayName);
        enumProperty.enumValueIndex = GUILayout.Toolbar(enumProperty.enumValueIndex, enumProperty.enumDisplayNames);
        EditorGUILayout.EndHorizontal();
    }

    // T�� Deep Copy�� ��ü�� Type
    public static void DeepCopySerializeReference(SerializedProperty property)
    {
        // managedReferenceValue�� SerializeReference Attribute�� ������ ����
        if (property.managedReferenceValue == null)
            return;

        property.managedReferenceValue = (property.managedReferenceValue as ICloneable).Clone();
    }

    public static void DeepCopySerializeReferenceArray(SerializedProperty property, string fieldName = "")
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            // Array���� Element�� ������
            var elementProperty = property.GetArrayElementAtIndex(i);
            // Element�� �Ϲ� class�� struct�� Element ���ο� SerializeReference ������ ���� �� �����Ƿ�,
            // fieldName�� Empty�� �ƴ϶�� Elenemt���� fieldName ���� ������ ã�ƿ�
            if (!string.IsNullOrEmpty(fieldName))
                elementProperty = elementProperty.FindPropertyRelative(fieldName);

            if (elementProperty.managedReferenceValue == null)
                continue;

            // ã�ƿ� ������ �̿��ؼ� property�� manageredRefenceValue���� Clone �Լ��� �����Ŵ
            elementProperty.managedReferenceValue = (elementProperty.managedReferenceValue as ICloneable).Clone();
        }
    }
}