using System;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Raytracing
{
    public class RayTracingMaster : MonoBehaviour
    {
        public event Action OnChanged;

        [Header("Graphics")]
        [SerializeField]
        private ComputeShader rayTracer;

        [SerializeField]
        private Texture skybox;

        [SerializeField]
        private OnRenderImageDispatcher onRenderImageDispatcher;

        [SerializeField]
        private Antialiasing antialiasing;

        [Header("Ray Tracing data")]
        [SerializeField, Range(0, 10)]
        private int bounces;

        [SerializeField]
        private bool castShadows = true;

        [SerializeField, Range(0.1f, 2f)]
        private float skyboxStrength = 1f;

        [SerializeField]
        private LightDataSource lightSource;

        [SerializeField]
        private SphereCreator sphereCreator;

        private new Camera camera;
        private RenderTexture renderTexture, converged;

        private int threadGroupsX, threadGroupsY;

        private DirectionalLight[] lightArray;
        private ComputeBuffer sphereBuffer, lightBuffer;

        private void Awake()
        {
            threadGroupsX = Mathf.CeilToInt(Screen.width / 32);
            threadGroupsY = Mathf.CeilToInt(Screen.height / 32);
            camera = Camera.main;

            CreateRenderTexture();
            rayTracer.SetTexture(0, "Skybox", skybox);

            onRenderImageDispatcher.OnImageRendered += OnImageRendered;

            lightArray = new DirectionalLight[1];
            lightBuffer = new ComputeBuffer(1, Marshal.SizeOf<DirectionalLight>());
            rayTracer.SetBuffer(0, "Light", lightBuffer);

            lightSource.OnLightUpdated += OnLightUpdated;
            sphereCreator.OnSpheresCreated += OnSpheresCreated;
        }

        private void OnDestroy()
        {
            renderTexture?.Release();
            converged?.Release();
            OnChanged = null;
        }

        private void OnValidate()
        {
            rayTracer.SetInt("Bounces", bounces);
            rayTracer.SetBool("CastShadows", castShadows);

            OnChanged?.Invoke();
        }

        private void OnImageRendered(RenderTexture source, RenderTexture destination)
        {
            SetShaderParameters();
            rayTracer.Dispatch(0, threadGroupsX, threadGroupsY, 1);
            Graphics.Blit(renderTexture, converged, antialiasing.GetMaterial());
            Graphics.Blit(converged, destination);
        }

        private void SetShaderParameters()
        {
            rayTracer.SetMatrix("CameraToWorld", camera.cameraToWorldMatrix);
            rayTracer.SetMatrix("CameraInverseProjection", camera.projectionMatrix.inverse);
            rayTracer.SetVector("PixelOffset", new Vector2(Random.value, Random.value));
            rayTracer.SetFloat("SkyboxStrength", skyboxStrength);
            rayTracer.SetFloat("Seed", Random.value);
        }

        private void CreateRenderTexture()
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            rayTracer.SetTexture(0, "Result", renderTexture);
            rayTracer.SetVector("TextureSize", new Vector2(renderTexture.width, renderTexture.height));

            converged = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }

        private void OnLightUpdated(DirectionalLight light)
        {
            lightArray[0] = light;
            lightBuffer.SetData(lightArray);
        }

        private void OnSpheresCreated(Sphere[] spheres)
        {
            sphereBuffer?.Release();

            sphereBuffer = new ComputeBuffer(spheres.Length, Marshal.SizeOf<Sphere>());
            sphereBuffer.SetData(spheres);
            rayTracer.SetBuffer(0, "Spheres", sphereBuffer);
            rayTracer.SetInt("SphereCount", spheres.Length);
        }
    }
}