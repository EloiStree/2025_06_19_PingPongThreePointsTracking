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

                Vector2 globalPixelCoordinate = localBlobTexture.m_globalSquareCenterCoordinate;

                GlobalPixelToUnityWorldQuad(width, height, m_leftDownRoot, m_rightDownCornerPoint, globalPixelCoordinate,
                    out Vector3 globalPosition, out Quaternion rotation);

                Vector2 globalPixelFarCoordinate = localBlobTexture.m_globalBorderMaxCoordinatePixel;
                GlobalPixelToUnityWorldQuad(width, height, m_leftDownRoot, m_rightDownCornerPoint, globalPixelFarCoordinate,
                    out Vector3 globalPositionFar, out Quaternion rotationFar);
                float deltaSize = Vector3.Distance(globalPosition, globalPositionFar)*2;

                if (index < m_blobCenterPoints.Length)
                {
                    m_blobCenterPoints[index].gameObject.SetActive(true);
                    m_blobCenterPoints[index].position = globalPosition;
                    m_blobCenterPoints[index].rotation = rotation;
                    m_blobCenterPoints[index].localScale = new Vector3(deltaSize, deltaSize, deltaSize);
                }
              

                index++;    
            }

        }

        private void GlobalPixelToUnityWorldQuad(
          int width,
          int height,
          Transform rootQuadStart,
          Transform corderQuadEnd,
          Vector2 globalPixelCoordinate,
          out Vector3 globalPosition,
          out Quaternion globalRotation)
        {
            globalPixelCoordinate.x /= width;
            globalPixelCoordinate.y /= height;


            globalRotation = rootQuadStart.rotation;
            Vector3 directionGlobal = corderQuadEnd.position - rootQuadStart.position;
            Vector3 directionLocal = Quaternion.Inverse(globalRotation) * directionGlobal;
            Vector3 localPosition = new Vector3(
                globalPixelCoordinate.x * directionLocal.x,
                globalPixelCoordinate.y * directionLocal.y,
                0f
            );
            globalPosition = (globalRotation * localPosition) + rootQuadStart.position;
            globalRotation = Quaternion.LookRotation(directionGlobal, Vector3.up);

        }



    }
}
