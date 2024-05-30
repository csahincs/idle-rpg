using System;
using System.Linq;
using System.Reflection;
using DependencyInjection.Runtime.Domain;
using DependencyInjection.Runtime.Utility;
using DependencyInjection.Utility.SerializableType;
using UnityEditor;
using UnityEngine;

namespace DependencyInjection.Editor
{
    [CustomEditor(typeof(SceneDependencyDomain))]
    public class DependencyDomainEditor : UnityEditor.Editor
    {
        private SerializedProperty _dependencyListProperty;
        
        private void OnEnable()
        {
            _dependencyListProperty = serializedObject.FindProperty("DependencyProviders");
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SceneDependencyDomain domain = (SceneDependencyDomain)target;
            
            // Show the interface list elements
            EditorGUILayout.Space();
            
            serializedObject.Update();
            for (var i = 0; i < _dependencyListProperty.arraySize; i++)
            {
                var typeProperty = _dependencyListProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true); // Make the field read-only
                EditorGUILayout.PropertyField(typeProperty, GUIContent.none);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    _dependencyListProperty.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            
            // Apply any changes to the serialized object
            serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Add Dependency"))
            {
                ShowAddInterfaceWindow(domain);
            }
        }

        private void ShowAddInterfaceWindow(SceneDependencyDomain domain)
        {
            GenericMenu menu = new GenericMenu();

            // Find all types in the project that implement IDependencyProvider
            var dependencyProviderType = typeof(IDependencyProvider);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => dependencyProviderType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();

            // Create the menu items in a foldered structure
            foreach (var type in types)
            {
                var namespacePath = type.Namespace != null ? type.Namespace.Replace('.', '/') : "NoNamespace";
                var menuPath = $"{namespacePath}/{type.Name}";
                var serializableType = new SerializableType(type);
                menu.AddItem(new GUIContent(menuPath), false, () => AddInterfaceInstance(serializableType, domain));
            }

            menu.ShowAsContext();
        }

        private void AddInterfaceInstance(SerializableType type, SceneDependencyDomain domain)
        {
            domain.DependencyProviders.Add(type);
        }
    }
}
