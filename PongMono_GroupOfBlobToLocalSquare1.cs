// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{
    public class  PongMono_GroupOfBlobToLocalSquare :MonoBehaviour
    {
        public GroupOfBlobPixelCountWithSource m_groupOfBlobs;
        public List<LocalBlobTexture> m_localBlobTextures = new List<LocalBlobTexture>();

        public UnityEvent<LocalBlobsTextureWithSource> m_onLocalSquareComputed = new UnityEvent<LocalBlobsTextureWithSource>();

        public void SetWithBlog(GroupOfBlobPixelCountWithSource blob) {

            m_groupOfBlobs = blob;
            m_localBlobTextures.Clear();
            foreach (BlobPixelCount blobPixelCount in m_groupOfBlobs.m_groupOfBlobs.m_blobPixelCounts)
            {
                LocalBlobTexture localBlobTexture = new LocalBlobTexture();
                localBlobTexture.SetWithBlobPixelCount(blobPixelCount);
                m_localBlobTextures.Add(localBlobTexture);
            }
            m_onLocalSquareComputed?.Invoke(new LocalBlobsTextureWithSource()
            {
                m_groupOfBlobs = m_groupOfBlobs,
                m_localBlobTextures = m_localBlobTextures
            });
        }
    }
}
