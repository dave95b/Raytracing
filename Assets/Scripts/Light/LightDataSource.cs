using System;
using UnityEngine;

namespace Raytracing
{
    public class LightDataSource : MonoBehaviour
    {
        public event Action<DirectionalLight> OnLightUpdated;

        [SerializeField]
        private Light directionalLight;

        private DirectionalLight currentData;

        private void Start()
        {
            DispatchNewData(CreateLightData());
        }

        private void Update()
        {
            var data = CreateLightData();
            if (currentData != data)
                DispatchNewData(data);
        }

        private void OnDestroy() => OnLightUpdated = null;

        private void DispatchNewData(in DirectionalLight data)
        {
            currentData = data;
            OnLightUpdated?.Invoke(currentData);
        }

        private DirectionalLight CreateLightData()
        {
            Vector3 direction = directionalLight.transform.forward;
            Vector4 color = directionalLight.color;
            float intensity = directionalLight.intensity;

            return new DirectionalLight(direction, color, intensity);
        }
    }
}