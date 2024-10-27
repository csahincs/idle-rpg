using UnityEngine;

namespace Common.DependencyInjection.Runtime.Domain
{
    [DefaultExecutionOrder(-2000)]
    public sealed class ProjectDependencyDomain : DependencyDomain
    {
        public static ProjectDependencyDomain Instance { get; private set; }
        
        protected override void Awake()
        {
            // If there is an instance, and it's not me, delete myself.
    
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
            
            DontDestroyOnLoad(gameObject);
            
            base.Awake();
        }
    }
}
