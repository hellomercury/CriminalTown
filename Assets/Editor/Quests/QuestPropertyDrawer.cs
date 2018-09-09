using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {
    
    [CustomPropertyDrawer(typeof(QuestAward), true)]
    public class QuestAwardPropertyDrawer : PropertyDrawer {
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Money"));
        }
    }
    
    [CustomPropertyDrawer(typeof(QuestTransition), true)]
    public class ChoiceQuestChoicePropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ShortDescription"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("NextId"));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Award"), true);
            EditorGUILayout.Separator();

        }
        
    }
    
    [CustomPropertyDrawer(typeof(Quest), true)]
    public class QuestPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Id"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Name"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Description"), GUILayout.Height(60));
        }

    }
    
    
    [CustomPropertyDrawer(typeof(ChoiceQuest), true)]
    public class ChoiceQuestPropertyDrawer : QuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Choices"), true);
        }

    }
    
    
    [CustomPropertyDrawer(typeof(LinearQuest), true)]
    public class LinearQuestPropertyDrawer : QuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("SuccessTransition"), true);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("FailTransition"), true);
        }
        
    }
    
    [CustomPropertyDrawer(typeof(CharacterQuest), true)]
    public class CharacterQuestPropertyDrawer : LinearQuestPropertyDrawer {
    }
    
    [CustomPropertyDrawer(typeof(StatsUpCharacterQuest), true)]
    public class StatsUpCharacterQuestPropertyDrawer : CharacterQuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("StatId"));
        }
        
    }
    
    [CustomPropertyDrawer(typeof(StatusCharacterQuest), true)]
    public class StatusCharacterQuestPropertyDrawer : CharacterQuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Status"));
        }
        
    }

    [CustomPropertyDrawer(typeof(ItemQuest), true)]
    public class ItemQuestPropertyDrawer : LinearQuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ItemId"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Count"));
        }
        
    }
    
    [CustomPropertyDrawer(typeof(RobberyQuest), true)]
    public class RobberyQuestPropertyDrawer : LinearQuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("RobberyType"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Count"));
        }
        
    }
    
    [CustomPropertyDrawer(typeof(EducationQuest), true)]
    public class EducationQuestPropertyDrawer : LinearQuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.LabelField("Add education system");
        }
        
    }

}