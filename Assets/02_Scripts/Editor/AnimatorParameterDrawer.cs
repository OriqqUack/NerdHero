using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AnimatorParameter))]
public class AnimatorParameterDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var nameProperty = property.FindPropertyRelative("name");
        var typeProperty = property.FindPropertyRelative("type");
        var indexProperty = property.FindPropertyRelative("index");
        var statProperty = property.FindPropertyRelative("stat");

        position = EditorGUI.PrefixLabel(position, label);

        float adjust = EditorGUI.indentLevel * 15f;

        // 각 파트 너비 계산
        float typeWidth = (position.width * 0.15f) + adjust;
        float indexWidth = typeWidth; // type과 같은 너비
        float nameWidth = position.width - typeWidth - indexWidth;

        // 각 Rect 배치
        var typeRect = new Rect(position.x, position.y, typeWidth - 2.5f, position.height);
        int enumInt = System.Convert.ToInt32(EditorGUI.EnumPopup(typeRect, (AnimatorParameterType)typeProperty.enumValueIndex));
        typeProperty.enumValueIndex = enumInt;

        var indexRect = new Rect(typeRect.x + typeRect.width + 2.5f, position.y, indexWidth - 2.5f, position.height);
        indexProperty.intValue = EditorGUI.IntField(indexRect, GUIContent.none, indexProperty.intValue);

        var statRect = new Rect(indexRect.x + indexRect.width + 2.5f, position.y, indexWidth - 2.5f, position.height);
        EditorGUI.PropertyField(statRect, statProperty, GUIContent.none, true);
        
        var nameRect = new Rect(statRect.x + statRect.width + 2.5f, position.y, nameWidth, position.height);
        nameProperty.stringValue = EditorGUI.TextField(nameRect, GUIContent.none, nameProperty.stringValue);

        EditorGUI.EndProperty();
    }

}
