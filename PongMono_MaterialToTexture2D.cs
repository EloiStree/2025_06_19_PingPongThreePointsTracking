// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{
    public class PongMono_MaterialToTexture2D : MonoBehaviour
    {
        public Material m_source;
        public Texture2D m_lastTextureCaptured;
        public UnityEvent<Texture2D> m_onTextureCaptured;
        public bool m_useApply = true;
        public void SetSource(Material source)
        {
            m_source = source;
        }
        [ContextMenu("Capture And Relay")]
        public void CaptureAndRelay()
        {
            if (m_source == null)
            {
                return;
            }
            if (m_lastTextureCaptured == null || m_lastTextureCaptured.width != m_source.mainTexture.width || m_lastTextureCaptured.height != m_source.mainTexture.height)
            {
                m_lastTextureCaptured = new Texture2D((int)m_source.mainTexture.width, (int)m_source.mainTexture.height, TextureFormat.RGBA32, false);
            }

            CopyMaterialToTexture(m_source, m_lastTextureCaptured);
            m_onTextureCaptured?.Invoke(m_lastTextureCaptured);
        }

        private void CopyMaterialToTexture(Material m_source, Texture2D m_lastTextureCaptured)
        {
            int width = m_lastTextureCaptured.width;
            int height = m_lastTextureCaptured.height;
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            // Render the material itself, not just its main texture
            Graphics.Blit(null, renderTexture, m_source);
            RenderTexture.active = renderTexture;
            m_lastTextureCaptured.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            if (m_useApply)
            {
                m_lastTextureCaptured.Apply();
            }
           
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }
}
