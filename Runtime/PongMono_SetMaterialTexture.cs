// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Eloi.PongTracking
{
    public class PongMono_SetMaterialTexture : MonoBehaviour
    {
        public Material m_material;
        
        public void SetTexture(Texture texture)
        {
            if (m_material == null)
            {
                return;
            }
            m_material.mainTexture = texture;
        }
        public void SetTexture(Texture2D texture)
        {
            if (m_material == null)
            {
                return;
            }
            m_material.mainTexture = texture;
        }
        public void SetTexture(WebCamTexture texture)
        {
            if (m_material == null)
            {
                return;
            }
            m_material.mainTexture = texture;
        }
        

    }
}
