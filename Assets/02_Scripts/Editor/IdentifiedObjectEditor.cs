using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(IdentifiedObject), true)]
public class IdentifiedObjectEditor : Editor
{
    // SerializedProperty�� ���� ���� �ִ� ��ü�� public Ȥ�� [SerializeField] ��Ʈ����Ʈ�� ����
    // Serailize�� �������� ���� �����ϱ� ���� class
    private SerializedProperty categoriesProperty;
    private SerializedProperty iconProperty;
    private SerializedProperty idProperty;
    private SerializedProperty codeNameProperty;
    private SerializedProperty displayNameProperty;
    private SerializedProperty descriptionProperty;

    // Inspector �󿡼� ������ ������ �� �ִ� List
    private ReorderableList categories;

    // text�� �а� �����ִ� Style(=Skin) ������ ���� ����
    private GUIStyle textAreaStyle;

    // Title�� Foldout Expand ���¸� �����ϴ� ����
    private readonly Dictionary<string, bool> isFoldoutExpandedesByTitle = new();

    protected virtual void OnEnable()
    {
        // Inspector���� description�� �����ϴٰ� �ٸ� Inspector View�� �Ѿ�� ��쿡,
        // ��Ŀ���� Ǯ���� �ʰ� ������ �����ϴ� desription ������ �״�� ���̴� ������ �ذ��ϱ����� ��Ŀ���� Ǯ����
        GUIUtility.keyboardControl = 0;

        // serializedObject�� ���� ���� Editor���� ���� �ִ� IdentifiedObject�� ����
        // ��ü���� Serialize �������� ã�ƿ�
        categoriesProperty = serializedObject.FindProperty("categories");
        iconProperty = serializedObject.FindProperty("icon");
        idProperty = serializedObject.FindProperty("id");
        codeNameProperty = serializedObject.FindProperty("codeName");
        displayNameProperty = serializedObject.FindProperty("displayName");
        descriptionProperty = serializedObject.FindProperty("description");

        // target ������ ���ؼ� ���� ���� Editor���� ���� �ִ� ���� IdentifiedObject ��ü�� ������ �� ����
        // var identifiedObject = target as IdentifiedObject
        // serializedObject�� targetObject ������ ���ؼ� ���� IdentifiedObject ��ü�� ������ �� ����
        // var identifiedObject = serializedObject.targetObject as IdentifieidObject;

        categories = new(serializedObject, categoriesProperty);
        // List�� Prefix Label�� ��� �׸��� ����D
        categories.drawHeaderCallback = rect => EditorGUI.LabelField(rect, categoriesProperty.displayName);
        // List�� Element�� ��� �׸��� ����
        categories.drawElementCallback = (rect, index, isActive, isFocused) => {
            rect = new Rect(rect.x, rect.y + 2f, rect.width, EditorGUIUtility.singleLineHeight);
            // EditorGUILayout�� EditorGUI�� ������
            // EditorGUILayout�� GUI�� �׸��� ������ ���� ��ġ�� �ڵ����� ��������
            // EditorGUI�� ����ڰ� ���� GUI�� �׸� ��ġ�� �����������
            EditorGUI.PropertyField(rect, categoriesProperty.GetArrayElementAtIndex(index), GUIContent.none);
        };
    }

    private void StyleSetup()
    {
        if (textAreaStyle == null)
        {
            // Style�� �⺻ ����� textArea.
            textAreaStyle = new(EditorStyles.textArea);
            // ���ڿ��� TextBox ������ �� ���������� ��.
            textAreaStyle.wordWrap = true;
        }
    }

    protected bool DrawFoldoutTitle(string text)
        => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedesByTitle, text);

    public override void OnInspectorGUI()
    {
        StyleSetup();

        // ��ü�� Serialize �������� ���� ������Ʈ��.
        serializedObject.Update();

        // List�� �׷���
        categories.DoLayoutList();

        if (DrawFoldoutTitle("Infomation"))
        {
            // (1) ���ݺ��� �׸� ��ü�� ���η� �����ϸ�, ����� �׵θ� �ִ� ȸ������ ä��(=HelpBox�� ����Ƽ ���ο� ���ǵǾ� �ִ� Style��)
            // �߰�ȣ�� �ۼ��� �ʿ�� ������ ��Ȯ�� ������ ���� �־��� ���̱� ������ ��Ÿ�Ͽ� ���� �߰�ȣ�� �ȳ־ ��.
            EditorGUILayout.BeginHorizontal("HelpBox");
            {
                //Sprite�� Preview�� �� �� �ְ� ������ �׷���
                iconProperty.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none, iconProperty.objectReferenceValue,
                    typeof(Sprite), false, GUILayout.Width(65));

                // (2) ���ݺ��� �׸� ��ü�� ���η� �����Ѵ�.
                // �� icon ������ ���ʿ� �׷�����, ���ݺ��� �׸� �������� �����ʿ� ���η� �׷���.
                EditorGUILayout.BeginVertical();
                {
                    // (3) ���ݺ��� �׸� ��ü�� ���η� �����Ѵ�.
                    // id ������ prefix(= inspector���� ���̴� ������ �̸�)�� ���� �������ֱ� ���� ���� Line�� ���� ����.
                    EditorGUILayout.BeginHorizontal();
                    {
                        // ���� ���� Disable, ID�� Database���� ���� Set���� ���̱� ������ ����ڰ� ���� �������� ���ϵ��� ��.
                        GUI.enabled = false;
                        // ������ ���� ��Ī(Prefix) ����
                        EditorGUILayout.PrefixLabel("ID");
                        // id ������ �׸��� Prefix�� �׸�������(=GUIContent.none); 
                        EditorGUILayout.PropertyField(idProperty, GUIContent.none);
                        // ���� ���� Enable
                        GUI.enabled = true;
                    }
                    // (3) ���� ���� ����
                    EditorGUILayout.EndHorizontal();

                    // ���ݺ��� ������ �����Ǿ����� �˻��Ѵ�.
                    EditorGUI.BeginChangeCheck();
                    var prevCodeName = codeNameProperty.stringValue;
                    // codeName ������ �׸���, ����ڰ� Enter Ű�� ���� ������ �� ������ ������.
                    EditorGUILayout.DelayedTextField(codeNameProperty);
                    // ������ �����Ǿ����� Ȯ��, codeName ������ �����Ǿ��ٸ� ������ ������ ���� ��ü�� �̸��� �ٲ���.
                    if (EditorGUI.EndChangeCheck())
                    {
                        // ���� ��ü�� ����Ƽ ������Ʈ���� �ּҸ� ������.
                        // target == IdentifiedObject, var identifiedObject = target as IdentifiecObject �̷� ������ ����� �� ����.
                        // serializeObject.targetObject == target
                        var assetPath = AssetDatabase.GetAssetPath(target);
                        // ���ο� �̸��� '(������ Type)_(codeName)'
                        var newName = $"{target.GetType().Name.ToUpper()}_{codeNameProperty.stringValue}";

                        // Serialize �������� �� ��ȭ�� ������(=��ũ�� ������)
                        // �� �۾��� ������ ������ �ٲ� ���� ������� �ʾƼ� ���� ������ ���ư�
                        serializedObject.ApplyModifiedProperties();

                        // ��ü�� Project View���� ���̴� �̸��� ������. ���� ���� �̸��� ���� ��ü�� ���� ��� ������.
                        var message = AssetDatabase.RenameAsset(assetPath, newName);
                        // �������� ��� ��ü�� ���� �̸��� �ٲ���. �ܺ� �̸��� ���� �̸��� �ٸ� �� ����Ƽ���� ��� ����,
                        // ���� ������Ʈ������ ������ ����ų ���ɼ��� ���⿡ �׻� �̸��� ��ġ���������
                        if (string.IsNullOrEmpty(message))
                            target.name = newName;
                        else
                            codeNameProperty.stringValue = prevCodeName;
                    }

                    // displayName ������ �׷���
                    EditorGUILayout.PropertyField(displayNameProperty);
                }
                // (2) ���� ���� ����
                EditorGUILayout.EndVertical();
            }
            // (1) ���� ���� ����
            EditorGUILayout.EndHorizontal();
            
            // ���� ���� ����, �⺻������ ���� ������ Default �����̱� ������ ���� ���� ���ο� ����ϴ°� �ƴ϶��
            // ���� ���� ������ ���� �ʿ䰡 ������ �� ��쿡�� HelpBox�� ���θ� ȸ������ ä������� ���� ���� ������ ��
            EditorGUILayout.BeginVertical("HelpBox");
            {
                // Description�̶�� Lebel�� �����
                EditorGUILayout.LabelField("Description");
                // TextField�� ���� ����(TextArea)�� �׷���
                descriptionProperty.stringValue = EditorGUILayout.TextArea(descriptionProperty.stringValue,
                    textAreaStyle, GUILayout.Height(60));
            }
            EditorGUILayout.EndVertical();
            // ���� ���� ����
        }

        // Serialize �������� �� ��ȭ�� ������(=��ũ�� ������)
        // �� �۾��� ������ ������ �ٲ� ���� ������� �ʾƼ� ���� ������ ���ư�
        serializedObject.ApplyModifiedProperties();
    }

    // Data�� Level�� Data ������ ���� X Button�� �׷��ִ� Foldout Title�� �׷���
    protected bool DrawRemovableLevelFoldout(SerializedProperty datasProperty, SerializedProperty targetProperty,
        int targetIndex, bool isDrawRemoveButton)
    {
        // Data�� �����ߴ����� ���� ���
        bool isRemoveButtonClicked = false;

        EditorGUILayout.BeginHorizontal();
        {
            GUI.color = Color.green;
            var level = targetProperty.FindPropertyRelative("level").intValue;
            // Data�� Level�� �����ִ� Foldout GUI�� �׷���
            targetProperty.isExpanded = EditorGUILayout.Foldout(targetProperty.isExpanded, $"Level {level}");
            GUI.color = Color.white;

            if (isDrawRemoveButton)
            {
                GUI.color = Color.red;
                if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    isRemoveButtonClicked = true;
                    // EffectDatas���� ���� Data�� Index�� �̿��� ����
                    datasProperty.DeleteArrayElementAtIndex(targetIndex);
                }
                GUI.color = Color.white;
            }
        }
        EditorGUILayout.EndHorizontal();

        return isRemoveButtonClicked;
    }

    protected void DrawAutoSortLevelProperty(SerializedProperty datasProperty, SerializedProperty levelProperty,
        int index, bool isEditable)
    {
        if (!isEditable)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(levelProperty);
            GUI.enabled = true;
        }
        else
        {
            // Property�� �����Ǿ����� ���� ����
            EditorGUI.BeginChangeCheck();
            // ������ Level�� ����ص�
            var prevValue = levelProperty.intValue;
            // levelProperty�� Delayed ������� �׷���
            // Ű���� Enter Key�� ������ �Է��� ���� �ݿ���, Enter Key�� �������ʰ� ���������� ���� ������ ���ƿ�.
            EditorGUILayout.DelayedIntField(levelProperty);
            // Property�� �����Ǿ��� ��� true ��ȯ
            if (EditorGUI.EndChangeCheck())
            {
                if (levelProperty.intValue <= 1)
                    levelProperty.intValue = prevValue;
                else
                {
                    // EffectDatas�� ��ȸ�Ͽ� ���� level�� ���� data�� �̹� ������ ���� �� level�� �ǵ���
                    for (int i = 0; i < datasProperty.arraySize; i++)
                    {
                        // Ȯ���ؾ��ϴ� Data�� ���� Data�� �����ϴٸ� Skip
                        if (index == i)
                            continue;

                        var element = datasProperty.GetArrayElementAtIndex(i);
                        // Level�� �Ȱ����� ���� Data�� Level�� ���� ������ �ǵ���
                        if (element.FindPropertyRelative("level").intValue == levelProperty.intValue)
                        {
                            levelProperty.intValue = prevValue;
                            break;
                        }
                    }

                    // Level�� ���������� �����Ǿ��ٸ� �������� ���� �۾� ����
                    if (levelProperty.intValue != prevValue)
                    {
                        // ���� Data�� Level�� i��° Data�� Level���� ������, ���� Data�� i��°�� �ű�
                        // ex. 1 2 4 5 (3) => 1 2 (3) 4 5
                        for (int moveIndex = 1; moveIndex < datasProperty.arraySize; moveIndex++)
                        {
                            if (moveIndex == index)
                                continue;

                            var element = datasProperty.GetArrayElementAtIndex(moveIndex).FindPropertyRelative("level");
                            if (levelProperty.intValue < element.intValue || moveIndex == (datasProperty.arraySize - 1))
                            {
                                datasProperty.MoveArrayElement(index, moveIndex);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
