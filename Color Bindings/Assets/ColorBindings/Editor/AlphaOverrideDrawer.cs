using UnityEditor;
using UnityEngine;

namespace ColorBindings.Editor
{
    [CustomPropertyDrawer(typeof(AlphaOverrideAttribute))]
    public class AlphaOverrideDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsVisible(property))
                EditorGUI.Slider(position, property, 0f, 1f, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Negative spacing removes the gap Unity adds between properties, so a hidden field takes up no space
            return IsVisible(property) ? EditorGUIUtility.singleLineHeight : -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool IsVisible(SerializedProperty property)
        {
            var alphaOverride = (AlphaOverrideAttribute)attribute;
            SerializedProperty toggle = property.serializedObject.FindProperty(alphaOverride.ToggleField);

            return toggle != null && toggle.boolValue;
        }
    }
}
