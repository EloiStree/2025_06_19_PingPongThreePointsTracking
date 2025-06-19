// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{
    public class PongMono_BasicFilterBlogs : MonoBehaviour
    {
        public GroupOfBlobPixelCountWithSource m_groupOfBlobs;

        public int m_minBlobPixelCount = 100;
        public float m_maxBlobPercent= 0.3f;

        public bool m_useTheBiggestBlobOnly = true;
        public int m_biggestBlobClampCount = 10;
        

        public UnityEvent<GroupOfBlobPixelCountWithSource> m_onBlobsFiltered = new UnityEvent<GroupOfBlobPixelCountWithSource>();

        public void SetAndProcessGroupOfBlobs(GroupOfBlobPixelCountWithSource groupOfBlobPixelCountWithSource) {

            m_groupOfBlobs = groupOfBlobPixelCountWithSource;
            m_groupOfBlobs.m_groupOfBlobs.m_blobPixelCounts.Sort((a, b) => b.m_blobPixelCount.CompareTo(a.m_blobPixelCount));
            m_groupOfBlobs.m_groupOfBlobs.m_blobPixelCounts.RemoveAll(blob => blob.m_blobPixelCount < m_minBlobPixelCount || blob.m_blobPixelCountRatio > m_maxBlobPercent);

            if (m_useTheBiggestBlobOnly)
            {
                if (m_groupOfBlobs.m_groupOfBlobs.m_blobPixelCounts.Count > m_biggestBlobClampCount)
                {
                    m_groupOfBlobs.m_groupOfBlobs.m_blobPixelCounts.RemoveRange(m_biggestBlobClampCount, m_groupOfBlobs.m_groupOfBlobs.m_blobPixelCounts.Count - m_biggestBlobClampCount);
                }
            }

            m_onBlobsFiltered.Invoke(m_groupOfBlobs);

        }
    }
}
