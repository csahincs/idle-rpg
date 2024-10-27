using Common.DependencyInjection.Runtime.Domain;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Common.DependencyInjection.Editor
{
    [CustomEditor(typeof(DependencyDomain), editorForChildClasses:true)]
    public class DependencyDomainEditor : UnityEditor.Editor
    {
        public VisualTreeAsset Uxml;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            Uxml.CloneTree(root);

            var mcListView = root.Q<MultiColumnListView>("injected");
            var domain = target as DependencyDomain;
            
            mcListView.itemsSource = domain.DependencyProviders;
            
            // Make Cell

            var cols = mcListView.columns;
            cols["type"].makeCell = () => new Label();
            cols["object"].makeCell = () => new ObjectField();
            cols["actions"].makeCell = () => new Button();
            
            // Bind Cell

            cols["type"].bindCell = (element, i) =>
            {
                (element as Label).text = domain.DependencyProviders[i].TypeName;
            };
            cols["object"].bindCell = (element, i) =>
            {
                (element as ObjectField).objectType = domain.DependencyProviders[i].Type;
                (element as ObjectField).allowSceneObjects = true;
                (element as ObjectField).SetEnabled(false);
            };
            cols["actions"].bindCell = (element, i) =>
            {
                (element as Button).RegisterCallback<MouseUpEvent>(RemoveDependency);
            };
            
            return root;
        }

        private void MakeCell()
        {
            
        }

        private void BindCell()
        {
            
        }

        private void RemoveDependency(MouseUpEvent evt)
        {
            
        }
    }
}
