using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {
    
    [CustomPropertyDrawer(typeof(QuestAward), true)]
    public class QuestAwardProperyDrawer : PropertyDrawer {
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Money"));
        }
    }
    
    [CustomPropertyDrawer(typeof(QuestTransition), true)]
    public class ChoiceQuestChoiceProperyDrawer : PropertyDrawer {
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
    public class ChoiceQuestProperyDrawer : QuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Choices"), true);
        }

    }
    
    
    [CustomPropertyDrawer(typeof(LinearQuest), true)]
    public class LinearQuestProperyDrawer : QuestPropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("SuccessTransition"), true);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("FailTransition"), true);
        }
        
    }
    
    [CustomPropertyDrawer(typeof(CharacterQuest), true)]
    public class CharacterQuestProperyDrawer : LinearQuestProperyDrawer {
    }
    
    [CustomPropertyDrawer(typeof(StatsUpCharacterQuest), true)]
    public class StatsUpCharacterQuestProperyDrawer : CharacterQuestProperyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("StatId"));
        }
        
    }
    
    [CustomPropertyDrawer(typeof(StatusCharacterQuest), true)]
    public class StatusCharacterQuestProperyDrawer : CharacterQuestProperyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Status"));
        }
        
    }

    [CustomPropertyDrawer(typeof(ItemQuest), true)]
    public class ItemQuestProperyDrawer : LinearQuestProperyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("ItemId"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Count"));
        }
        
    }
    
    [CustomPropertyDrawer(typeof(RobberyQuest), true)]
    public class RobberyQuestProperyDrawer : LinearQuestProperyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.PropertyField(property.FindPropertyRelative("RobberyType"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("Count"));
        }
        
    }
    
    [CustomPropertyDrawer(typeof(EducationQuest), true)]
    public class EducationQuestProperyDrawer : LinearQuestProperyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
            EditorGUILayout.LabelField("Add education system");
        }
        
    }

}