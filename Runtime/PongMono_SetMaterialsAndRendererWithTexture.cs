// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Eloi.PongTracking
{
    public class PongMono_SetMaterialsAndRendererWithTexture : MonoBehaviour
    {
        public Texture2D m_texture;
        public Renderer [] m_renderer;
        public Material[] m_material;
        public void SetTexture(Texture2D texture)
        {
            if (m_renderer == null || m_material == null)
            {
                return;
            }
            m_texture = texture;
            foreach (var rend in m_renderer)
            {
                if (rend != null)
                {
                    rend.material.mainTexture = m_texture;
                }
            }
            foreach (var mat in m_material)
            {
                if (mat != null)
                {
                    mat.mainTexture = m_texture;
                }
            }
        }
    }
}
