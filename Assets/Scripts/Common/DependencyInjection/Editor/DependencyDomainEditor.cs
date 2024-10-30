using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.DependencyInjection.Attributes;
using Common.DependencyInjection.Runtime.Domain;
using Common.DependencyInjection.Utility.SerializableType;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Common.DependencyInjection.Editor
{
    [CustomEditor(typeof(DependencyDomain), editorForChildClasses:true)]
    public class DependencyDomainEditor : UnityEditor.Editor
    {
        public VisualTreeAsset Uxml;
        
        private DependencyDomain _domain;
        private List<SerializableType> _allPossibleProviders;
        

        private DropdownField _providersDropdownField;
        private MultiColumnListView _multiColumnListView;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            Uxml.CloneTree(root);
            
            #region Current Dependencies

            _multiColumnListView = root.Q<MultiColumnListView>("injected");
            _domain = target as DependencyDomain;
            
            _multiColumnListView.itemsSource = _domain.DependencyProviders;
            
            // Make Cell

            var cols = _multiColumnListView.columns;
            cols["type"].makeCell = () => new Label();
            cols["object"].makeCell = () => new ObjectField();
            cols["actions"].makeCell = () => new Button() { text = "Remove" };
            
            // Bind Cell

            cols["type"].bindCell = (element, i) =>
            {
                (element as Label).text = _domain.DependencyProviders[i].Type.Name;
            };
            cols["object"].bindCell = (element, i) =>
            {
                (element as ObjectField).objectType = _domain.DependencyProviders[i].Type;
                (element as ObjectField).allowSceneObjects = true;
                (element as ObjectField).SetEnabled(false);
            };
            cols["actions"].bindCell = (element, i) =>
            {
                (element as Button).RegisterCallback<MouseUpEvent>(_ => RemoveDependency(i));
            };

            #endregion

            #region Add Dependencies

            _allPossibleProviders = GetTypesWithAttribute<ProviderAttribute>();
            var typeNames = _allPossibleProviders.ConvertAll(type => type.Type.Name);
            
            _providersDropdownField = new DropdownField("Select Provider", typeNames, 0);
            var button = new Button(AddDependency) { text = "Add" };

            var addDependencyElement = root.Q<VisualElement>("add_dependency");
            addDependencyElement.Add(_providersDropdownField);
            addDependencyElement.Add(button);

            #endregion
            
            return root;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _multiColumnListView.itemsSource = _domain.DependencyProviders;
            _multiColumnListView.Rebuild();
        }

        private void AddDependency()
        {
            _domain.DependencyProviders.Add(_allPossibleProviders[_providersDropdownField.index]);
        }

        private void RemoveDependency(int index)
        {
            _domain.DependencyProviders.RemoveAt(index);
        }

        #region Utility

        private static List<SerializableType> GetTypesWithAttribute<T>() where T : ProviderAttribute
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttribute<T>() != null)
                .Select(t => new SerializableType(t))
                .ToList();
        }

        #endregion
    }
}
