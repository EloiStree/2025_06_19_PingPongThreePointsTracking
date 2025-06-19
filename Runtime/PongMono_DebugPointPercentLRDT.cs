// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;

namespace Eloi.PongTracking
{
    public class PongMono_DebugPointPercentLRDT : MonoBehaviour
    {
        public LocalBlobsTextureWithSource m_localBlobsTextureWithSource;
        public Transform m_leftDownRoot;
        public Transform m_rightDownCornerPoint;
        public Transform[] m_blobCenterPoints;


        [ContextMenu("RefreshWithInspectorDate")]
        public void RefreshWithInspectorDate()
        {
            if (m_localBlobsTextureWithSource != null)
            {
                SetWithBlog(m_localBlobsTextureWithSource);
            }
        }


        public void SetWithBlog(LocalBlobsTextureWithSource localBlobsTextureWithSource)
        {
            m_localBlobsTextureWithSource = localBlobsTextureWithSource;
            int index = 0;

            foreach (Transform blobCenterPoint in m_blobCenterPoints)
            {
                blobCenterPoint.gameObject.SetActive(false);
            }

            foreach (LocalBlobTexture localBlobTexture in m_localBlobsTextureWithSource.m_localBlobTextures)
            {

                Texture2D t = m_localBlobsTextureWithSource.m_groupOfBlobs.m_texture;
                int width = t.width;
                int height = t.height;
                Vector2 centerLeftRight = localBlobTexture.m_globalSquareCenterCoordinate;
                centerLeftRight.x /= width;
                centerLeftRight.y /= height;


                Quaternion rotation = m_leftDownRoot.rotation;
                Vector3 directionGlobal = m_rightDownCornerPoint.position - m_leftDownRoot.position;
                Vector3 directionLocal = Quaternion.Inverse(rotation) * directionGlobal;
                Vector3 localPosition = new Vector3(
                    centerLeftRight.x * directionLocal.x,
                    centerLeftRight.y * directionLocal.y,
                    0f
                );
                Vector3 globalPosition = (rotation * localPosition) + m_leftDownRoot.position;
               
                if (index < m_blobCenterPoints.Length)
                {
                    m_blobCenterPoints[index].gameObject.SetActive(true);
                    m_blobCenterPoints[index].position = globalPosition;
                    m_blobCenterPoints[index].rotation = rotation;
                }
              

                index++;    
            }

        }

    }
}
