using System;
using UnityEngine;

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

        [SerializeField]
        private int seed;

        private Sphere[] spheres;

        private RNG rng;

        private void Awake()
        {
            rng = new RNG(seed);
        }

        private void OnEnable() => CreateSpheres();

        private void OnDestroy() => OnSpheresCreated = null;

        [ContextMenu("Create")]
        private void CreateSpheres()
        {
            if (spheres?.Length != sphereCount)
                spheres = new Sphere[sphereCount];

            for (int i = 0; i < sphereCount; i++)
            {
                float radius = rng.Float(minRadius, maxRadius);
                Vector3 position = rng.InsideUnitCircle * placementRadius;
                position.z = position.y;
                position.y = radius;

                Color color = rng.ColorHSV(0f, 1f, 0.5f, 1, 0.5f, 1.0f);

                Vector3 fromColor = new Vector3(color.r, color.g, color.b);
                float metallic = rng.Float(0.2f, 1f);
                Vector3 albedo = fromColor * metallic;
                Vector3 specular = fromColor * (1f - metallic);

                spheres[i] = new Sphere(position, radius, albedo, specular);
            }

            OnSpheresCreated?.Invoke(spheres);
        }
    }
}