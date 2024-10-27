using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.DependencyInjection.Attributes;
using Common.DependencyInjection.Runtime.Container;
using Common.DependencyInjection.Utility.SerializableType;
using UnityEngine;

namespace Common.DependencyInjection.Runtime.Domain
{
    public class DependencyDomain : MonoBehaviour
    {
        [SerializeField, HideInInspector] public List<SerializableType> DependencyProviders = new();

        private readonly DependencyContainer _dependencyContainer = new();
        
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        #region Register Dependencies

        public void Register<TService>()
        {
            _dependencyContainer.Register<TService>();
        }
        
        public void Register(Type type)
        {
            _dependencyContainer.Register(type);
        }

        public void Register(object implementation)
        {
            _dependencyContainer.Register(implementation);
        }

        #endregion
        
        #region Inject Dependencies

        private void Inject(object instance) 
        {
            var type = instance.GetType();
            
            // Inject into fields
            var injectableFields = type.GetFields(BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableField in injectableFields) {
                if (injectableField.GetValue(instance) != null) {
                    Debug.LogWarning($"[Injector] Field '{injectableField.Name}' of class '{type.Name}' is already set.");
                    continue;
                }
                var fieldType = injectableField.FieldType;
                var resolvedInstance = _dependencyContainer.Resolve(fieldType);
                if (resolvedInstance == null) {
                    throw new Exception($"Failed to inject dependency into field '{injectableField.Name}' of class '{type.Name}'.");
                }
                
                injectableField.SetValue(instance, resolvedInstance);
            }
            
            // Inject into methods
            var injectableMethods = type.GetMethods(BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableMethod in injectableMethods) {
                var requiredParameters = injectableMethod.GetParameters()
                    .Select(parameter => parameter.ParameterType)
                    .ToArray();
                var resolvedInstances = requiredParameters.Select(_dependencyContainer.Resolve).ToArray();
                if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null)) {
                    throw new Exception($"Failed to inject dependencies into method '{injectableMethod.Name}' of class '{type.Name}'.");
                }
                
                injectableMethod.Invoke(instance, resolvedInstances);
            }
            
            // Inject into properties
            var injectableProperties = type.GetProperties(BINDING_FLAGS)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
            foreach (var injectableProperty in injectableProperties) {
                var propertyType = injectableProperty.PropertyType;
                var resolvedInstance = _dependencyContainer.Resolve(propertyType);
                if (resolvedInstance == null) {
                    throw new Exception($"Failed to inject dependency into property '{injectableProperty.Name}' of class '{type.Name}'.");
                }

                injectableProperty.SetValue(instance, resolvedInstance);
            }
        }

        #endregion

        #region Utility
        
        private static MonoBehaviour[] FindMonoBehaviours() {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }

        private static bool IsInjectable(MonoBehaviour obj) {
            var members = obj.GetType().GetMembers(BINDING_FLAGS);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }

        #endregion

        protected virtual void Awake()
        {
            for (var i = 0; i < DependencyProviders.Count; i++)
            {
                var type = DependencyProviders[i].Type;
                
                _dependencyContainer.Register(type);
            }

            var monoBehaviours = FindMonoBehaviours();
            
            // Find all injectable objects and inject their dependencies
            var injectables = monoBehaviours.Where(IsInjectable);
            foreach (var injectable in injectables) 
            {
                Inject(injectable);
            }
        }
    }
}
