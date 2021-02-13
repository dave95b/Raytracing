using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class OnRenderImageDispatcher : MonoBehaviour
    {
        public event Action<RenderTexture> OnImageRendered;

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            OnImageRendered?.Invoke(dest);
        }

        private void OnDestroy()
        {
            OnImageRendered = null;
        }
    }
}