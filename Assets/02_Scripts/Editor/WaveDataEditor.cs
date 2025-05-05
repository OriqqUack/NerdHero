using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SOWaveData))]
public class SOWaveDataEditor : Editor
{
    private SerializedProperty wavesProperty;
    private GUIStyle headerStyle;
    private GUIStyle subHeaderStyle;
    private GUIStyle boxStyle;
    private List<bool> waveFoldouts = new List<bool>();
    private List<List<bool>> subWaveFoldouts = new List<List<bool>>();
    private bool stylesInitialized = false;

    private void OnEnable()
    {
        wavesProperty = serializedObject.FindProperty("Waves");
        EnsureFoldoutListSize();
    }

    private void InitializeStyles()
    {
        if (!stylesInitialized)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                normal = { textColor = Color.cyan }
            };

            subHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                normal = { textColor = Color.green }
            };

            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 5, 5)
            };

            stylesInitialized = true;
        }
    }

    private void EnsureFoldoutListSize()
    {
        while (waveFoldouts.Count < wavesProperty.arraySize)
        {
            waveFoldouts.Add(false);
            subWaveFoldouts.Add(new List<bool>());
        }
    }

    public override void OnInspectorGUI()
    {
        InitializeStyles();
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🌊 Wave Editor", headerStyle);
        EditorGUILayout.Space();

        if (GUILayout.Button("+ Add Wave", GUILayout.Height(30)))
        {
            wavesProperty.arraySize++;
            waveFoldouts.Add(false);
            subWaveFoldouts.Add(new List<bool>());
        }

        EnsureFoldoutListSize();

        for (int i = 0; i < wavesProperty.arraySize; i++)
        {
            SerializedProperty waveProperty = wavesProperty.GetArrayElementAtIndex(i);
            SerializedProperty enemiesProperty = waveProperty.FindPropertyRelative("Enemies");

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(boxStyle);

            waveFoldouts[i] = EditorGUILayout.Foldout(waveFoldouts[i], $"🌊 Wave {i + 1}", true, headerStyle);
            
            if (waveFoldouts[i])
            {
                EditorGUI.indentLevel++;

                if (GUILayout.Button("+ Add Sub Wave", GUILayout.Height(25)))
                {
                    enemiesProperty.arraySize++;
                    subWaveFoldouts[i].Add(false);
                }

                while (subWaveFoldouts[i].Count < enemiesProperty.arraySize)
                {
                    subWaveFoldouts[i].Add(false);
                }

                for (int j = 0; j < enemiesProperty.arraySize; j++)
                {
                    SerializedProperty enemyProperty = enemiesProperty.GetArrayElementAtIndex(j);
                    SerializedProperty enemyLevelProperty = enemyProperty.FindPropertyRelative("EnemyLevel");
                    SerializedProperty enemyPrefabProperty = enemyProperty.FindPropertyRelative("EnemyPrefab");
                    SerializedProperty enemyCountProperty = enemyProperty.FindPropertyRelative("EnemyCount");

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(boxStyle);
                    subWaveFoldouts[i][j] = EditorGUILayout.Foldout(subWaveFoldouts[i][j], $"🌀 SubWave {j + 1}", true, subHeaderStyle);

                    if (subWaveFoldouts[i][j])
                    {
                        for (int k = 0; k < enemyPrefabProperty.arraySize; k++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(enemyLevelProperty.GetArrayElementAtIndex(k), GUIContent.none, GUILayout.Width(50));
                            EditorGUILayout.PropertyField(enemyPrefabProperty.GetArrayElementAtIndex(k), GUIContent.none, GUILayout.Width(150));
                            EditorGUILayout.PropertyField(enemyCountProperty.GetArrayElementAtIndex(k), GUIContent.none, GUILayout.Width(50));

                            if (GUILayout.Button("❌", GUILayout.Width(30)))
                            {
                                enemyLevelProperty.DeleteArrayElementAtIndex(k);
                                enemyPrefabProperty.DeleteArrayElementAtIndex(k);
                                enemyCountProperty.DeleteArrayElementAtIndex(k);
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        if (GUILayout.Button("+ Add Enemy", GUILayout.Height(22)))
                        {
                            enemyLevelProperty.arraySize++;
                            enemyPrefabProperty.arraySize++;
                            enemyCountProperty.arraySize++;
                        }

                        if (GUILayout.Button("🗑 Remove SubWave", GUILayout.Height(20)))
                        {
                            enemiesProperty.DeleteArrayElementAtIndex(j);
                            subWaveFoldouts[i].RemoveAt(j);
                            break;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("🗑 Remove Wave", GUILayout.Height(25)))
                {
                    wavesProperty.DeleteArrayElementAtIndex(i);
                    waveFoldouts.RemoveAt(i);
                    subWaveFoldouts.RemoveAt(i);
                    break;
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}