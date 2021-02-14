using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Raytracing
{
    public class SphereCreator : MonoBehaviour
    {
        public event Action<Sphere[]> OnSpheresCreated;

        [SerializeField, Range(0.5f, 2f)]
        private float minRadius = 1;

        [SerializeField, Range(2f, 6f)]
        private float maxRadius = 3;

        [SerializeField]
        private int sphereCount = 25;

        [SerializeField]
        private float placementRadius = 30.0f;

        private Sphere[] spheres;

        private void Start() => CreateSpheres();

        private void OnEnable() => CreateSpheres();

        private void OnDestroy() => OnSpheresCreated = null;

        private void CreateSpheres()
        {
            if (spheres?.Length != sphereCount)
                spheres = new Sphere[sphereCount];

            for (int i = 0; i < sphereCount; i++)
            {
                float radius = Random.Range(minRadius, maxRadius);
                Vector3 position = Random.insideUnitCircle * placementRadius;
                position.z = position.y;
                position.y = radius;

                Color color = Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1.0f);

                Vector3 fromColor = new Vector3(color.r, color.g, color.b);
                float metallic = Random.Range(0.2f, 1f);
                Vector3 albedo = fromColor * metallic;
                Vector3 specular = fromColor * (1f - metallic);

                spheres[i] = new Sphere(position, radius, albedo, specular);
            }

            OnSpheresCreated?.Invoke(spheres);
        }
    }
}