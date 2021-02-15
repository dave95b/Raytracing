﻿using System;
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

        [SerializeField, Range(0f, 1f)]
        private float metalChance, emissiveChance;

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

                Sphere sphere;

                Color color = rng.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1.5f);
                Vector3 colorVec = FromColor(color);
                float smoothness = rng.Float(2f);

                if (rng.Float() < emissiveChance)
                    sphere = CreateEmissiveSphere(position, radius);
                else if (rng.Float() < metalChance)
                    sphere = Sphere.Metal(position, radius, smoothness, colorVec);
                else
                    sphere = Sphere.Soft(position, radius, smoothness, colorVec);

                spheres[i] = sphere;
            }

            OnSpheresCreated?.Invoke(spheres);
        }

        private Sphere CreateEmissiveSphere(in Vector3 position, float radius)
        {
            Color color = rng.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 2f);
            Vector3 emission = FromColor(color);

            return Sphere.Emissive(position, radius, emission);
        }

        private Vector3 FromColor(in Color color) => new Vector3(color.r, color.g, color.b);
    }
}