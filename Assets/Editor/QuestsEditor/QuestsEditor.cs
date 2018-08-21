using System.Collections.Generic;
using CriminalTown.Serialization;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {
    
    [CustomEditor(typeof(QuestsScriptableObject), true)]
    public class QuestsScriptableObjectDrawer : Editor {
        public override void OnInspectorGUI() {
            if (GUILayout.Button("Open quests editor")) {
                QuestsEditor.ShowQuestsEditor();
            }
        }
    }

    public class QuestsEditor : EditorWindow {
        private static QuestsEditor m_instance;
        private static QuestsScriptableObject m_questsCollection;
        private static GUIStyle m_titleStyle;

        private static Vector2 m_scroll;
        private static SerializedObject m_questsObject;
        private static List<SerializedProperty> m_questsProperties;
        private static bool[] m_questTypeFoldout;


        [MenuItem("CriminalTown/Quests editor")]
        public static void ShowQuestsEditor() {
            m_questsCollection = QuestsSerialization.GetQuestsCollection;
            if (m_questsCollection == null) {
                Debug.LogError("Can not find quest data!");
                return;
            }

            if (m_instance == null) {
                m_instance = GetWindow<QuestsEditor>();
            }
            m_instance.Show();
        }

        private void OnEnable() {
            m_scroll = Vector2.zero;
            
            m_questsObject = new SerializedObject(m_questsCollection);
            m_questsProperties = new List<SerializedProperty> {
                m_questsObject.FindProperty("m_hireCharacterQuests"),
                m_questsObject.FindProperty("m_kickCharacterQuests"),
                m_questsObject.FindProperty("m_statusCharacterQuests"),
                m_questsObject.FindProperty("m_levelUpCharacterQuests"),
                m_questsObject.FindProperty("m_statsUpCharacterQuests"),
                m_questsObject.FindProperty("m_itemQuests"),
                m_questsObject.FindProperty("m_robberyQuests"),
                m_questsObject.FindProperty("m_educationQuests"),
                m_questsObject.FindProperty("m_choiceQuests"),
            };
            
            m_questTypeFoldout = new bool[m_questsProperties.Count];

            m_titleStyle = new GUIStyle() {
                alignment = TextAnchor.UpperCenter,
                fontSize = 25,
                fontStyle = FontStyle.Bold,
            };

            minSize = new Vector2(400, 300);
        }

        private void OnGUI() {
            EditorGUILayout.LabelField("Quests", m_titleStyle);
            m_scroll = GUILayout.BeginScrollView(m_scroll);
            {
                for (var quPropNum = 0; quPropNum < m_questsProperties.Count; quPropNum++) {
                    SerializedProperty questsProperty = m_questsProperties[quPropNum];
                    int questsCount = questsProperty.arraySize;

                    m_questTypeFoldout[quPropNum] = EditorGUILayout.Foldout(m_questTypeFoldout[quPropNum], questsProperty.name);
                    if (m_questTypeFoldout[quPropNum]) {
                        for (int i = 0; i < questsCount; i++) {
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.PropertyField(questsProperty.GetArrayElementAtIndex(i), true);
                            EditorGUILayout.EndVertical();
                            if (GUILayout.Button("Remove quest")) {
                                if (EditorUtility.DisplayDialog("Delete quest", "A you sure you want to delete quest: questId", "Yes", "No")) {
                                    questsProperty.DeleteArrayElementAtIndex(i);
                                    break; //avoid OutOfRange
                                }
                            }
                        }
                        if (GUILayout.Button("Add quest")) {
                            questsProperty.InsertArrayElementAtIndex(questsCount);
                        }
                    }
                }
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
//          if (GUILayout.Button("Save (doesn't work yet)")) {
//              
//          }
            GUILayout.EndHorizontal();

            m_questsObject.ApplyModifiedProperties();
            if (GUI.changed) {
                EditorUtility.SetDirty(m_questsCollection);
            }
        }
        
    }

}