using Assets.Scripts.Utils;
using UnityEngine;

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

        private new Camera camera;
        private RenderTexture renderTexture;

        private int threadGroupsX, threadGroupsY;

        private Material antialiasingMaterial;
        private float antialiasingSample = 0.0f;

        private void Start()
        {
            threadGroupsX = Mathf.CeilToInt(Screen.width / 32);
            threadGroupsY = Mathf.CeilToInt(Screen.height / 32);
            camera = Camera.main;

            CreateRenderTexture();
            rayTracer.SetTexture(0, "Skybox", skybox);

            onRenderImageDispatcher.OnImageRendered += OnImageRendered;

            antialiasingMaterial = new Material(antialiasingShader);
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
                Vector3 l = directionalLight.transform.forward;
                rayTracer.SetVector("DirectionalLight", new Vector4(l.x, l.y, l.z, directionalLight.intensity));
                directionalLight.transform.hasChanged = false;
            }
        }

        private void OnValidate()
        {
            antialiasingSample = 0;
            rayTracer.SetInt("Bounces", bounces);
            rayTracer.SetBool("CastShadows", castShadows);
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
        }
    }
}