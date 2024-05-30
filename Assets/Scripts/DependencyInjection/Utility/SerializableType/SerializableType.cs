using System;
using UnityEngine;

namespace DependencyInjection.Utility.SerializableType
{
    [Serializable]
    public class SerializableType
    {
        [SerializeField] private string TypeName;

        public Type Type
        {
            get => Type.GetType(TypeName);
            set => TypeName = value.Name;
        }

        public SerializableType(Type type)
        {
            Type = type;
        }
    }
}