using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SOWaveData))]
public class WaveDataEditor : Editor
{
    private SerializedProperty waveListProperty;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        waveListProperty = serializedObject.FindProperty("Waves");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        WaveEditing();

        serializedObject.ApplyModifiedProperties();
    }

    private void WaveEditing()
    {
        EditorGUILayout.LabelField("Wave Data", EditorStyles.boldLabel);

        if (GUILayout.Button("➕ Add New Wave", GUILayout.Height(25)))
        {
            waveListProperty.arraySize++;
            SerializedProperty newWave = waveListProperty.GetArrayElementAtIndex(waveListProperty.arraySize - 1);
            newWave.FindPropertyRelative("Wave").intValue = waveListProperty.arraySize;
            newWave.FindPropertyRelative("Enemies").arraySize = 0;
        }

        // 스크롤뷰 시작
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));

        for (int i = 0; i < waveListProperty.arraySize; i++)
        {
            SerializedProperty wave = waveListProperty.GetArrayElementAtIndex(i);
            SerializedProperty waveNumber = wave.FindPropertyRelative("Wave");
            SerializedProperty enemiesList = wave.FindPropertyRelative("Enemies");

            if (wave == null || waveNumber == null || enemiesList == null)
            {
                continue; // null 값이 있으면 스킵
            }

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"🌊 Wave {i + 1}", EditorStyles.boldLabel);
            waveNumber.intValue = i + 1;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enemy", GUILayout.Width(150));
            EditorGUILayout.LabelField("Count", GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            for (int j = 0; j < enemiesList.arraySize; j++)
            {
                SerializedProperty enemyEntry = enemiesList.GetArrayElementAtIndex(j);
                SerializedProperty enemyPrefab = enemyEntry.FindPropertyRelative("EnemyPrefab");
                SerializedProperty enemyCount = enemyEntry.FindPropertyRelative("EnemyCount");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(enemyPrefab, GUIContent.none, GUILayout.Width(150));
                EditorGUILayout.PropertyField(enemyCount, GUIContent.none, GUILayout.Width(70));

                if (GUILayout.Button("❌", GUILayout.Width(30)))
                {
                    enemiesList.DeleteArrayElementAtIndex(j);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("➕ Add Enemy"))
            {
                enemiesList.arraySize++;
            }

            if (GUILayout.Button("🗑 Remove Wave", GUILayout.Height(20)))
            {
                waveListProperty.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        // 스크롤뷰 종료
        EditorGUILayout.EndScrollView();
    }
}
