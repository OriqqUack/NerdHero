using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SOWaveData))]
public class WaveDataEditor : Editor
{
    private SerializedProperty waveListProperty;

    private void OnEnable()
    {
        waveListProperty = serializedObject.FindProperty("waves"); // WaveManager 안의 List<WaveData> 연결
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

            SerializedProperty enemyNames = newWave.FindPropertyRelative("EnemyPrefab");
            SerializedProperty enemyCounts = newWave.FindPropertyRelative("EnemyCount");

            enemyNames.arraySize++;
            enemyCounts.arraySize++;
        }

        for (int i = 0; i < waveListProperty.arraySize; i++)
        {
            SerializedProperty wave = waveListProperty.GetArrayElementAtIndex(i);
            SerializedProperty waveNumber = wave.FindPropertyRelative("Wave");
            SerializedProperty enemyNames = wave.FindPropertyRelative("EnemyPrefab");
            SerializedProperty enemyCounts = wave.FindPropertyRelative("EnemyCount");

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
    }
}
