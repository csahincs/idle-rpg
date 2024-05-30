using System;
using UnityEngine;

namespace DependencyInjection.Utility.SerializableType
{
    [Serializable]
    public class SerializableType
    {
        [SerializeField] private string TypeName;

        private Type _type;
        public Type Type
        {
            get => _type;
            set
            {
                _type = value;
                TypeName = value.AssemblyQualifiedName;
            }
        }

        public SerializableType(Type type)
        {
            Type = type;
        }
    }
}