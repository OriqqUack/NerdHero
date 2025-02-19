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
        waveListProperty = serializedObject.FindProperty("Waves"); // 필드명이 정확한지 확인
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

            // newWave가 존재하는지 확인 후 초기화
            if (newWave != null)
            {
                SerializedProperty enemyNames = newWave.FindPropertyRelative("EnemyPrefab");
                SerializedProperty enemyCounts = newWave.FindPropertyRelative("EnemyCount");

                if (enemyNames != null) enemyNames.arraySize = 0;
                if (enemyCounts != null) enemyCounts.arraySize = 0;
            }
        }

        // 스크롤뷰 시작
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));

        for (int i = 0; i < waveListProperty.arraySize; i++)
        {
            SerializedProperty wave = waveListProperty.GetArrayElementAtIndex(i);
            SerializedProperty waveNumber = wave.FindPropertyRelative("Wave");
            SerializedProperty enemyNames = wave.FindPropertyRelative("EnemyPrefab");
            SerializedProperty enemyCounts = wave.FindPropertyRelative("EnemyCount");

            if (wave == null || waveNumber == null || enemyNames == null || enemyCounts == null)
            {
                continue; // null 값이 있으면 스킵
            }

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"🌊 Wave {i + 1}", EditorStyles.boldLabel);
            waveNumber.intValue = i + 1;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enemy Name", GUILayout.Width(150));
            EditorGUILayout.LabelField("Enemy Count", GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            for (int j = 0; j < enemyNames.arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(enemyNames.GetArrayElementAtIndex(j), GUIContent.none, GUILayout.Width(150));
                EditorGUILayout.PropertyField(enemyCounts.GetArrayElementAtIndex(j), GUIContent.none, GUILayout.Width(70));

                if (GUILayout.Button("❌", GUILayout.Width(30)))
                {
                    enemyNames.DeleteArrayElementAtIndex(j);
                    enemyCounts.DeleteArrayElementAtIndex(j);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("➕ Add Enemy"))
            {
                enemyNames.arraySize++;
                enemyCounts.arraySize++;
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
