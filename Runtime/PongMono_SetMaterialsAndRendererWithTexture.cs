// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Eloi.PongTracking
{
    public class PongMono_SetMaterialsAndRendererWithTexture : MonoBehaviour
    {
        public Renderer [] m_renderer;
        public Material[] m_material;

        public void Reset()
        {
            m_renderer = GetComponentsInChildren<Renderer>(true);
          
        }

        public void SetTexture(WebCamTexture texture)
        {

            if (m_renderer == null || m_material == null)
            {
                return;
            }
            foreach (var rend in m_renderer)
            {
                if (rend != null)
                {
                    rend.material.mainTexture = texture;
                }
            }
            foreach (var mat in m_material)
            {
                if (mat != null)
                {
                    mat.mainTexture = texture;
                }
            }
        }
        public void SetTexture(Texture2D texture)
        {
            if (m_renderer == null || m_material == null)
            {
                return;
            }
            foreach (var rend in m_renderer)
            {
                if (rend != null)
                {
                    rend.material.mainTexture = texture;
                }
            }
            foreach (var mat in m_material)
            {
                if (mat != null)
                {
                    mat.mainTexture = texture;
                }
            }
        }
    }
}
