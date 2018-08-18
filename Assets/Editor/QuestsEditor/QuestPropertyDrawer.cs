using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {

    [CustomPropertyDrawer(typeof(Quest))]
    public class QuestPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
           
            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.LabelField(label);
            EditorGUI.indentLevel++;
            
            Rect idRect = new Rect(position.x, position.y + 18, position.width, 16);
            Rect nameRect = new Rect(position.x, position.y + 36, position.width, 16);
            Rect descriptionRect = new Rect(position.x, position.y + 54, position.width, 48);

            EditorGUI.PropertyField(idRect, property.FindPropertyRelative("Id"));
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("Name"));
            EditorGUI.PropertyField(descriptionRect, property.FindPropertyRelative("Description"));

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 100f;
        }
    }

}