using DependencyInjection.Attributes;
using InputService2D;
using UnityEngine;

namespace Playground
{
    public class DependencyInjectionTesting : MonoBehaviour
    {
        [Inject] private InputService2D.InputService2D _inputService2D;

        private void Awake()
        {
            Debug.LogError(_inputService2D is null);
        }
    }
}
