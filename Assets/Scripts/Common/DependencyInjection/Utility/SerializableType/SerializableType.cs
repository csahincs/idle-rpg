using System;
using UnityEngine;

namespace Common.DependencyInjection.Utility.SerializableType
{
    [Serializable]
    public class SerializableType
    {
        [field: SerializeField] public string TypeName { get; private set; }

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