using System;
using System.Collections.Generic;
using System.Linq;
using Common.DependencyInjection.Runtime.Utility;

namespace Common.DependencyInjection.Runtime.Container
{
    public class DependencyContainer
    {
        private readonly Dictionary<Type, DependencyDescriptor> _registry = new ();

        #region Register Dependencies

        public void Register<TService>()
        {
            _registry.Add(typeof(TService), new DependencyDescriptor(DependencyLifetime.Singleton));
        }
        
        public void Register(Type type)
        {
            _registry.Add(type, new DependencyDescriptor(DependencyLifetime.Singleton));
        }

        public void Register(object implementation)
        {
            _registry.Add(implementation.GetType(), new DependencyDescriptor(implementation, DependencyLifetime.Singleton));
        }

        #endregion

        #region Resolve Dependency

        public object Resolve(Type type)
        {
            _registry.TryGetValue(type, out var dependency);

            if (dependency == null)
            {
                throw new Exception($"Service of type {type.Name} isn't registered");
            }
            
            if (dependency.Implementation != null)
            {
                return dependency.Implementation;
            }

            if (type.IsAbstract || type.IsInterface)
            {
                throw new Exception("Cannot instantiate abstract classes or interfaces");
            }

            var constructorInfo = type.GetConstructors().First();

            var parameters = constructorInfo.GetParameters()
                .Select(x => Resolve(x.ParameterType)).ToArray();

            var implementation = Activator.CreateInstance(type, parameters);

            if (dependency.Lifetime == DependencyLifetime.Singleton)
            {
                dependency.Implementation = implementation;    
            }
            
            return implementation;
        }

        #endregion
    }
}
