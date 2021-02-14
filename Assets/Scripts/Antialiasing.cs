using UnityEngine;

namespace Raytracing
{
    public class Antialiasing : MonoBehaviour
    {
        [SerializeField]
        private Shader antialiasingShader;

        [SerializeField]
        private ChangeDispatcher changeDispatcher;

        private Material antialiasingMaterial;
        private float antialiasingSample = 0.0f;

        private new Camera camera;

        private void Start()
        {
            antialiasingMaterial = new Material(antialiasingShader);
            camera = Camera.main;

            changeDispatcher.OnChanged += ResetSample;
        }

        private void Update()
        {
            if (!camera.transform.hasChanged)
                return;

            ResetSample();
            camera.transform.hasChanged = false;
        }

        public Material GetMaterial()
        {
            antialiasingMaterial.SetFloat("_Sample", antialiasingSample++);
            return antialiasingMaterial;
        }

        private void ResetSample() => antialiasingSample = 0;
    }
}