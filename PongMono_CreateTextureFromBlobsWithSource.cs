// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{
    public class PongMono_CreateTextureFromBlobsWithSource : MonoBehaviour

    {
        public GroupOfBlobPixelCountWithSource m_groupOfBlobs;
        public Texture2D m_texture;

        public UnityEvent<Texture2D> m_onTextureCreated = new UnityEvent<Texture2D>();


        public void SetAndBuildFrom(GroupOfBlobPixelCountWithSource source)
        {
            m_groupOfBlobs = source;
            if (source == null || source.m_texture == null)
            {
                return;
            }
            if (m_texture == null || m_texture.width != source.m_texture.width || m_texture.height != source.m_texture.height)
            {
                m_texture = new Texture2D(source.m_texture.width, source.m_texture.height, TextureFormat.RGBA32, false);
            }

            for (int i = 0; i < m_texture.width; i++)
            {
                for (int j = 0; j < m_texture.height; j++)
                {
                    m_texture.SetPixel(i, j, Color.clear);
                }
            }
            foreach (var blob in m_groupOfBlobs.m_groupOfBlobs.m_blobPixelCounts)
            {
                Color blobColor = new Color(Random.value, Random.value, Random.value, 1f);
                if (blob.m_blobPixels.Count > 0)
                {
                    foreach (var pixel in blob.m_blobPixels)
                    {
                        m_texture.SetPixel(pixel.x, pixel.y, blobColor);
                    }
                }
            }
            m_texture.Apply();
            m_onTextureCreated.Invoke(m_texture);
        }
    }
}
