using CriminalTown.Serialization;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {

    [CustomEditor(typeof(QuestsSerialization), true)]
    public class QuestSerializationDrawer : Editor {
        public override void OnInspectorGUI() {
            if (GUILayout.Button("Open quests editor")) {
                QuestsEditor.ShowQuestsEditor();
            }
        }
    }
    
    public class QuestsEditor : EditorWindow {
        private static QuestsEditor m_instance;
        private static QuestsSerialization m_questsCollection;
        private GUIStyle m_titleStyle;
        private Vector2 m_scroll;

        [MenuItem("CriminalTown/Quests editor")]
        public static void ShowQuestsEditor() {
            m_questsCollection = QuestsSerialization.Instance;
            if (m_questsCollection == null) {
                Debug.LogError("You must open Map scene!\n(or scene which contains QuestsSerialization.cs script)");
                return;
            }
            
            if (m_instance == null) {
                m_instance = GetWindow<QuestsEditor>();
            }
            m_instance.Show();
        }

        private void OnEnable() {
            m_scroll = Vector2.zero;
            
            m_titleStyle = new GUIStyle() {
                alignment = TextAnchor.UpperCenter,
                fontSize = 25,
                fontStyle = FontStyle.Bold,
            };
            minSize = new Vector2(200, 300);
        }

        private void OnDisable() {
            
        }

        private void OnGUI() {
            int verticalPosition = 0;
            SerializedObject questsObject = new SerializedObject(m_questsCollection);
            SerializedProperty questsProperty = questsObject.FindProperty("m_quests");
            
            GUILayout.Label("Quests", m_titleStyle);
            verticalPosition += 30;
            m_scroll = GUILayout.BeginScrollView(m_scroll);
            {
                for (int i = 0; i < questsProperty.arraySize; i++) {
                    SerializedProperty quest = questsProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.PropertyField(quest, true);
                    verticalPosition += (int)EditorGUI.GetPropertyHeight(quest, true);
                    //GUILayout.
                    if (GUILayout.Button("Remove quest")) {
                        if (EditorUtility.DisplayDialog("Delete quest", "A you sure you want to delete quest: questId", "Yes", "No")) {
                            m_questsCollection.Quests.RemoveAt(i);
                        }
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(30);
                }
            }
            GUILayout.EndScrollView();
            
            GUILayout.BeginHorizontal();
//            if (GUILayout.Button("Save (doesn't work yet)")) {
//                
//            }
            if (GUILayout.Button("Add quest")) {
                m_questsCollection.Quests.Add(new Quest());
            }
            GUILayout.EndHorizontal();
        }

    }

}