// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{
    public class PongMono_DebugTextureLocalBlobSquare : MonoBehaviour
    {
        public LocalBlobsTextureWithSource m_localBlobsTextureWithSource;
        public Texture2D m_texture;
        public UnityEvent<Texture2D> m_onTextureComputed;
        public void SetWithBlog(LocalBlobsTextureWithSource localBlobsTextureWithSource)
        {
            m_localBlobsTextureWithSource = localBlobsTextureWithSource;
            m_texture = new Texture2D(localBlobsTextureWithSource.m_groupOfBlobs.m_texture.width, localBlobsTextureWithSource.m_groupOfBlobs.m_texture.height, TextureFormat.RGBA32, false);
            m_texture.filterMode = FilterMode.Point;
            m_texture.wrapMode = TextureWrapMode.Clamp;


            for (int y = 0; y < m_texture.height; y++)
            {
                for (int x = 0; x < m_texture.width; x++)
                {
                    m_texture.SetPixel(x, y, Color.clear);
                }
            }

            foreach (LocalBlobTexture localBlobTexture in m_localBlobsTextureWithSource.m_localBlobTextures)
            {
                Texture2D refTexture = localBlobTexture.m_texture;
                int xOffset = localBlobTexture.m_globalLeftX;
                int yOffset = localBlobTexture.m_globalBottomY;
                int height = refTexture.height;
                for (int y = 0; y < refTexture.height; y++)
                {
                    for (int x = 0; x < refTexture.width; x++)
                    {
                        Color pixelColor = refTexture.GetPixel(x, y);
                        m_texture.SetPixel(x + xOffset, y + yOffset - height, pixelColor);
                    }
                }

            }
            m_texture.Apply();
            if (m_onTextureComputed != null)
            {
                m_onTextureComputed.Invoke(m_texture);
            }
        }
    }
}
