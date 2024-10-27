namespace Common.DependencyInjection.Runtime.Utility
{
    public class DependencyDescriptor
    {
        public object Implementation { get; internal set; }

        public DependencyLifetime Lifetime { get; }
        
        public DependencyDescriptor(DependencyLifetime lifetime)
        {
            Lifetime = lifetime;
        }
        
        public DependencyDescriptor(object implementation, DependencyLifetime lifetime)
        {
            Implementation = implementation;
            Lifetime = lifetime;
        }
    }
}
