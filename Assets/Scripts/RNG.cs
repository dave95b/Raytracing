using System;
using UnityEngine;
using Random = System.Random;

namespace Raytracing
{
    public class RNG
    {
        private readonly Random random;

        public RNG() : this(Guid.NewGuid().GetHashCode()) { }

        public RNG(int seed) => random = new Random(seed);

        public float Float() => Float(0f, 1f);
        public float Float(float max) => Float(0, max);
        public float Float(float min, float max) => (float)random.NextDouble() * (max - min) + min;

        public Vector2 OnUnitCircle => new Vector2(Float(-1f, 1f), Float(-1f, 1f)).normalized;
        public Vector2 InsideUnitCircle => Vector2(Float());
        public Vector2 Vector2(float magnitude) => OnUnitCircle * magnitude;

        public Vector3 OnUnitSphere => new Vector3(Float(-1f, 1f), Float(-1f, 1f), Float(-1f, 1f)).normalized;
        public Vector3 InsideUnitSphere => Vector3(Float());
        public Vector3 Vector3(float magnitude) => OnUnitSphere * magnitude;

        public Color ColorHSV() => ColorHSV(0f, 1f, 0f, 1f, 0f, 1f);
        public Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax)
        {
            float hue = Float(hueMin, hueMax);
            float saturation = Float(saturationMin, saturationMax);
            float value = Float(valueMin, valueMax);

            return Color.HSVToRGB(hue, saturation, value);
        }
    }
}
