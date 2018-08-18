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
        private static GUIStyle m_titleStyle;

        private static Vector2 m_scroll;
        private static int[] m_popups;

        [MenuItem("CriminalTown/Quests editor")]
        public static void ShowQuestsEditor() {
            m_questsCollection = QuestsSerialization.Instance;
            if (m_questsCollection == null) {
                Debug.LogError("You must open Map scene!\n(or scene which contains QuestsSerialization.cs script)");
                return;
            }

            m_popups = new int[m_questsCollection.Quests.Count];
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
            SerializedObject questsObject = new SerializedObject(m_questsCollection);
            SerializedProperty questsProperty = questsObject.FindProperty("m_quests");

            GUILayout.Label("Quests", m_titleStyle);
            m_scroll = GUILayout.BeginScrollView(m_scroll);
            {
                for (int i = 0; i < questsProperty.arraySize; i++) {
                    ManageQuest(questsProperty.GetArrayElementAtIndex(i), i);
                    GUILayout.Space(30);
                }
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
//          if (GUILayout.Button("Save (doesn't work yet)")) {
//              
//          }
            if (GUILayout.Button("Add quest")) {
                m_questsCollection.Quests.Add(new Quest());
            }
            GUILayout.EndHorizontal();

            questsObject.ApplyModifiedProperties();
        }

        private void ManageQuest(SerializedProperty quest, int index) {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_popups[index] = EditorGUILayout.Popup(m_popups[index], new string[] {
                "RobberyQuest",
                "ItemQuest",
                "StatsUpCharacterQuest",
                "LevelUpCharacterQuest",
                "StatusCharacterQuest",
                "KickCharacterQuest",
                "HireCharacterQuest",
                "ChoiceQuest",
            });
            EditorGUILayout.PropertyField(quest, true);
            if (GUILayout.Button("Remove quest")) {
                if (EditorUtility.DisplayDialog("Delete quest", "A you sure you want to delete quest: questId", "Yes", "No")) {
                    m_questsCollection.Quests.RemoveAt(index);
                }
            }
            EditorGUILayout.EndVertical();
        }

    }

}