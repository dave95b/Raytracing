using Assets.Scripts.Utils;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Assets.Scripts
{
    public class RayTracingMaster : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader rayTracer;

        [SerializeField]
        private OnRenderImageDispatcher onRenderImageDispatcher;

        [SerializeField]
        private Texture skybox;

        [SerializeField]
        private Shader antialiasingShader;

        [SerializeField]
        private Light directionalLight;

        [SerializeField, Range(0, 10)]
        private int bounces;

        [SerializeField]
        private bool castShadows = true;

        [Header("Sphere placement")]
        [SerializeField, Range(0.5f, 2f)]
        private float minRadius;

        [SerializeField, Range(2f, 6f)]
        private float maxRadius;

        [SerializeField]
        private int sphereCount = 25;

        [SerializeField]
        private float placementRadius = 100.0f;

        private new Camera camera;
        private RenderTexture renderTexture;

        private int threadGroupsX, threadGroupsY;

        private Material antialiasingMaterial;
        private float antialiasingSample = 0.0f;

        private ComputeBuffer sphereBuffer, lightBuffer;

        private void Awake()
        {
            threadGroupsX = Mathf.CeilToInt(Screen.width / 32);
            threadGroupsY = Mathf.CeilToInt(Screen.height / 32);
            camera = Camera.main;

            CreateRenderTexture();
            rayTracer.SetTexture(0, "Skybox", skybox);

            onRenderImageDispatcher.OnImageRendered += OnImageRendered;

            antialiasingMaterial = new Material(antialiasingShader);

            lightBuffer = new ComputeBuffer(1, Marshal.SizeOf<DirectionalLight>());
            rayTracer.SetBuffer(0, "Light", lightBuffer);
        }

        private void OnDestroy()
        {
            renderTexture?.Release();
        }

        private void Update()
        {
            if (camera.transform.hasChanged)
            {
                antialiasingSample = 0;
                camera.transform.hasChanged = false;
            }

            if (directionalLight.transform.hasChanged)
            {
                antialiasingSample = 0;
                directionalLight.transform.hasChanged = false;

                Vector3 direction = directionalLight.transform.forward;
                Vector4 color = directionalLight.color;

                var arr = new[] { new DirectionalLight(direction, color, directionalLight.intensity) };
                lightBuffer.SetData(arr);
            }
        }

        private void OnValidate()
        {
            antialiasingSample = 0;
            rayTracer.SetInt("Bounces", bounces);
            rayTracer.SetBool("CastShadows", castShadows);
        }

        private void OnEnable()
        {
            antialiasingSample = 0;
            CreateSpheres();
        }

        private void OnDisable()
        {
            sphereBuffer?.Release();
        }

        private void OnImageRendered(RenderTexture destination)
        {
            SetShaderParameters();
            rayTracer.Dispatch(0, threadGroupsX, threadGroupsY, 1);

            antialiasingMaterial.SetFloat("_Sample", antialiasingSample++);

            Graphics.Blit(renderTexture, destination, antialiasingMaterial);
        }

        private void SetShaderParameters()
        {
            rayTracer.SetMatrix("CameraToWorld", camera.cameraToWorldMatrix);
            rayTracer.SetMatrix("CameraInverseProjection", camera.projectionMatrix.inverse);
            rayTracer.SetVector("PixelOffset", new Vector2(Random.value, Random.value));
        }

        private void CreateRenderTexture()
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            rayTracer.SetTexture(0, "Result", renderTexture);
            rayTracer.SetVector("TextureSize", new Vector2(renderTexture.width, renderTexture.height));
        }

        private void CreateSpheres()
        {
            Sphere[] spheres = new Sphere[sphereCount];

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

            sphereBuffer = new ComputeBuffer(sphereCount, Marshal.SizeOf<Sphere>());
            sphereBuffer.SetData(spheres);
            rayTracer.SetBuffer(0, "Spheres", sphereBuffer);
            rayTracer.SetInt("SphereCount", sphereCount);
        }
    }
}