using System;
using UnityEngine;

namespace Raytracing
{
    public readonly struct DirectionalLight : IEquatable<DirectionalLight>
    {
        public readonly Vector3 Direction;
        public readonly Vector4 Color;
        public readonly float Intensity;

        public DirectionalLight(Vector3 direction, Vector4 color, float intensity)
        {
            Direction = direction;
            Color = color;
            Intensity = intensity;
        }

        public override bool Equals(object obj) => obj is DirectionalLight light && Equals(light);

        public bool Equals(DirectionalLight other) => Direction.Equals(other.Direction) &&
                   Color.Equals(other.Color) &&
                   Intensity == other.Intensity;

        public override int GetHashCode()
        {
            int hashCode = 941905676;
            hashCode = hashCode * -1521134295 + Direction.GetHashCode();
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            hashCode = hashCode * -1521134295 + Intensity.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DirectionalLight left, DirectionalLight right) => left.Equals(right);

        public static bool operator !=(DirectionalLight left, DirectionalLight right) => !(left == right);
    }
}