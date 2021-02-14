using System;
using UnityEngine;

namespace Raytracing
{
    public class ChangeDispatcher : MonoBehaviour
    {
        public event Action OnChanged;

        [SerializeField]
        private RayTracingMaster rayTracing;

        [SerializeField]
        private LightDataSource lightSource;

        [SerializeField]
        private SphereCreator sphereCreator;

        private void Start()
        {
            rayTracing.OnChanged += Dispatch;
            lightSource.OnLightUpdated += l => Dispatch();
            sphereCreator.OnSpheresCreated += s => Dispatch();
        }

        private void Dispatch() => OnChanged?.Invoke();
    }
}