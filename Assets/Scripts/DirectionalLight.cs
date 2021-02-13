using UnityEngine;

public readonly struct DirectionalLight
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
}
