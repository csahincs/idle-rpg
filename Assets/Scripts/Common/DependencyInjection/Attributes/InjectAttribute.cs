using System;

namespace Common.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property 
                    | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute { }
}
