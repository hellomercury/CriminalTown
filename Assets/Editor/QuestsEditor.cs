using UnityEngine;
using UnityEditor;
using System.IO;
using CriminalTown.Serialization;

public class GameDataEditor : EditorWindow {
    public QuestsCollection QuestsCollection;

    [MenuItem("Criminal Town/Quests Editor")]
    private static void OpenQuestEditor() {
        GetWindow(typeof(GameDataEditor)).Show();
    }

    private void OnGUI() {
        if (QuestsCollection != null) {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("QuestsCollection");
            //EditorGUILayout.PropertyField(serializedProperty, true);

            //serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save data")) {
                SaveGameData();
            }
        }

        if (GUILayout.Button("Load data")) {
            LoadGameData();
        }
    }

    private void LoadGameData() {
        QuestsCollection = QuestsSerialization.Deserialize();

    }

    private void SaveGameData() {
        QuestsSerialization.Serialize(QuestsCollection);
    }
}