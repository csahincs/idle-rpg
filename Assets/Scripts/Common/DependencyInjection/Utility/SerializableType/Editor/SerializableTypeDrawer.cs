using System;
using UnityEditor;
using UnityEngine;

namespace Common.DependencyInjection.Utility.SerializableType.Editor
{
    [CustomPropertyDrawer(typeof(global::Common.DependencyInjection.Utility.SerializableType.SerializableType))]
    public class SerializableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var typeNameProperty = property.FindPropertyRelative("TypeName");

            if (typeNameProperty != null)
            {
                // Display the type name as a label
                EditorGUI.LabelField(position, label.text, 
                    Type.GetType(typeNameProperty.stringValue)?.Name ?? "None");
            }

            EditorGUI.EndProperty();
        }
    }
}