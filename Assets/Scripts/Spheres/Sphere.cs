using System;
using System.Collections.Generic;
using UnityEngine;

namespace Raytracing
{
    public readonly struct Sphere
    {
        public readonly Vector3 Position;
        public readonly float Radius, Smoothness;
        public readonly Vector3 Albedo, Specular, Emission;

        public static Sphere Emissive(in Vector3 position, float radius, in Vector3 emission) => new Sphere(position, radius, 0f, Vector3.one, Vector3.one, emission);

        public static Sphere Metal(in Vector3 position, float radius, float smoothness, in Vector3 specular) => new Sphere(position, radius, smoothness, specular * 0.1f, specular, Vector3.zero);

        public static Sphere Soft(in Vector3 position, float radius, float smoothness, in Vector3 albedo) => new Sphere(position, radius, smoothness, albedo, Vector3.one * 0.1f, Vector3.zero);

        private Sphere(
            in Vector3 position,
            float radius,
            float smoothness,
            in Vector3 albedo,
            in Vector3 specular,
            in Vector3 emission)
        {
            Position = position;
            Radius = radius;
            Smoothness = smoothness;
            Albedo = albedo;
            Specular = specular;
            Emission = emission;
        }
    }
}