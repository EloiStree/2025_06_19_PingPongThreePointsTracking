// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{


    [System.Serializable]
    public class GroupOfBlobPixelCount
    {
        public List<BlobPixelCount> m_blobPixelCounts = new List<BlobPixelCount>();
    }


    [System.Serializable]
    public class LocalBlobTexture {
    
        public BlobPixelCount m_blobPixelCount;
        public Texture2D m_localTextureOfBlob;
        public int m_globalLeftPixelX;
        public int m_globalRightPixelX;
        public int m_globalTopPixelY;
        public int m_globalBottomPixelY;

        public float m_globalLeftPercentX;
        public float m_globalRightPercentX;
        public float m_globalTopPercentY;
        public float m_globalBottomPercentY;

        public int m_localTextureWidth;
        public int m_localTextureHeight;


       

        public Vector2Int m_localSquareCenterCoordinatePixel;
        public Vector2Int m_globalSquareCenterCoordinate;
        public Vector2Int m_globalPixelAverageCoordinate;
        public Vector2Int m_localPixelAverageCoordinate;

        public List<Vector2Int> m_emptyAroundFromBorder = new List<Vector2Int>();
        public List<Vector2Int> m_borderPixels = new List<Vector2Int>();
        public List<Vector2Int> m_innerBorder = new List<Vector2Int>();

        public float m_localBorderRadiusMin;
        public float m_localBorderRadiusMax;

        public Vector2Int m_localBorderMinCoordinatePixel;
        public Vector2Int m_localBorderMaxCoordinatePixel;
        public Vector2Int m_globalBorderMinCoordinatePixel;
        public Vector2Int m_globalBorderMaxCoordinatePixel;

        [Range(0,1)]
        public float m_percentMinCompareToMaxRadius;
        public bool m_isProbablyCircle;

        public void SetWithBlobPixelCount(BlobPixelCount blobPixelCount)
        {
            m_blobPixelCount = blobPixelCount;
            m_globalLeftPixelX = int.MaxValue;
            m_globalRightPixelX = int.MinValue;
            m_globalTopPixelY = int.MaxValue;
            m_globalBottomPixelY = int.MinValue;
            foreach (Vector2Int pixel in blobPixelCount.m_blobPixels)
            {
                if (pixel.x < m_globalLeftPixelX) m_globalLeftPixelX = pixel.x;
                if (pixel.x > m_globalRightPixelX) m_globalRightPixelX = pixel.x;
                if (pixel.y < m_globalTopPixelY) m_globalTopPixelY = pixel.y;
                if (pixel.y > m_globalBottomPixelY) m_globalBottomPixelY = pixel.y;
            }
            m_localTextureWidth = m_globalRightPixelX - m_globalLeftPixelX + 1;
            m_localTextureHeight = m_globalBottomPixelY - m_globalTopPixelY + 1;
            m_localSquareCenterCoordinatePixel = new Vector2Int( m_localTextureWidth / 2,  m_localTextureHeight / 2);
            m_globalSquareCenterCoordinate = new Vector2Int(m_globalLeftPixelX + m_localSquareCenterCoordinatePixel.x, m_globalTopPixelY + m_localSquareCenterCoordinatePixel.y);

            m_globalPixelAverageCoordinate = Vector2Int.zero;
            Vector2Int centreOfMassDirectionFromCenter = Vector2Int.zero;
            foreach (Vector2Int pixel in blobPixelCount.m_blobPixels)
            {
                centreOfMassDirectionFromCenter += pixel - m_globalSquareCenterCoordinate;
                m_globalPixelAverageCoordinate += pixel;
            }
            if (blobPixelCount.m_blobPixels.Count > 0)
            {
                m_globalPixelAverageCoordinate /= blobPixelCount.m_blobPixels.Count;
            }
            else
            {
                m_globalPixelAverageCoordinate = m_globalSquareCenterCoordinate;
            }

            m_localPixelAverageCoordinate = m_localSquareCenterCoordinatePixel + centreOfMassDirectionFromCenter / blobPixelCount.m_blobPixels.Count;

            m_localTextureOfBlob = new Texture2D(m_localTextureWidth, m_localTextureHeight);
            m_localTextureOfBlob.filterMode = FilterMode.Point;
            m_localTextureOfBlob.wrapMode = TextureWrapMode.Clamp;
            Color32[] localPixels = new Color32[m_localTextureWidth * m_localTextureHeight];
            Color32 borderColor = new Color32(255, 0, 0, 255); // Red for the border
            for (int i = 0; i < localPixels.Length; i++)
            {
                localPixels[i] = new Color32(0, 0, 0, 0); // Transparent
               

            }
            foreach (Vector2Int pixel in blobPixelCount.m_blobPixels)
            {
                int localX = pixel.x - m_globalLeftPixelX;
                int localY = pixel.y - m_globalTopPixelY;
                if (localX >= 0 && localX < m_localTextureWidth && localY >= 0 && localY < m_localTextureHeight)
                {
                    localPixels[localY * m_localTextureWidth + localX] = new Color32(255, 255, 255, 255); // White for blob pixels
                }
            }
          

            Color32 squareCenter = new Color32(0, 255, 255, 255);
            Color32 squareMassCenter = new Color32(0, 255, 0, 255);
            localPixels[m_localSquareCenterCoordinatePixel.y * m_localTextureWidth + m_localSquareCenterCoordinatePixel.x] = squareCenter; // Cyan for local center
            localPixels[m_localPixelAverageCoordinate.y * m_localTextureWidth + m_localPixelAverageCoordinate.x] = squareMassCenter; // Green for local average center
           


            List<Vector2Int> borderStartPoint = new List<Vector2Int>();
            for (int y = 0; y < m_localTextureHeight; y++)
            {
                borderStartPoint.Add(new Vector2Int(0, y)); // Left border
                borderStartPoint.Add(new Vector2Int(m_localTextureWidth - 1, y)); // Right border
            }
            for (int x = 0; x < m_localTextureWidth; x++)
            {
                borderStartPoint.Add(new Vector2Int(x, 0)); // Top border
                borderStartPoint.Add(new Vector2Int(x, m_localTextureHeight - 1)); // Bottom border
            }
            m_emptyAroundFromBorder = new List<Vector2Int>();
            List<Vector2Int> toExplore = new List<Vector2Int>(borderStartPoint);
            while (toExplore.Count > 0)
            {
                Vector2Int current = toExplore[0];
                toExplore.RemoveAt(0);
                if (current.x < 0 || current.x >= m_localTextureWidth || current.y < 0 || current.y >= m_localTextureHeight)
                {
                    continue; // Out of bounds
                }
                int index = current.y * m_localTextureWidth + current.x;
                bool isBlack = localPixels[index].r == 0 && localPixels[index].g == 0 && localPixels[index].b == 0;
                if (isBlack)
                {
                    m_emptyAroundFromBorder.Add(current);
                    localPixels[index] = new Color32(255, 255, 0, 255); // Yellow for empty around border
                    // Check neighbors
                    toExplore.Add(new Vector2Int(current.x - 1, current.y)); // Left
                    toExplore.Add(new Vector2Int(current.x + 1, current.y)); // Right
                    toExplore.Add(new Vector2Int(current.x, current.y - 1)); // Down
                    toExplore.Add(new Vector2Int(current.x, current.y + 1)); // Up

                    bool hasWhiteNeighbor = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue; // Skip the current pixel
                            int neighborX = current.x + dx;
                            int neighborY = current.y + dy;
                            if (neighborX >= 0 && neighborX < m_localTextureWidth && neighborY >= 0 && neighborY < m_localTextureHeight)
                            {
                                int neighborIndex = neighborY * m_localTextureWidth + neighborX;
                                if (localPixels[neighborIndex].r == 255 && localPixels[neighborIndex].g == 255 && localPixels[neighborIndex].b == 255)
                                {
                                    hasWhiteNeighbor = true;
                                    break;
                                }
                            }
                        }
                        if (hasWhiteNeighbor) break;
                    }
                    if ( hasWhiteNeighbor)
                    {
                        localPixels[index] = new Color32(0, 0, 255, 255); // Blue for empty around border with white neighbor

                        m_borderPixels.Add(current); // Add to border pixels
                    }
                }
            }

            m_localBorderRadiusMin = float.MaxValue;
            m_localBorderRadiusMax = float.MinValue;

            foreach (Vector2Int borderPixel in m_borderPixels)
            {
                Vector2Int direction = borderPixel - m_localSquareCenterCoordinatePixel;
                float distanceFromCenter = direction.magnitude;
                if (distanceFromCenter < m_localBorderRadiusMin)
                {
                    m_localBorderRadiusMin = distanceFromCenter;
                    m_localBorderMinCoordinatePixel = borderPixel;
                }
                if (distanceFromCenter > m_localBorderRadiusMax)
                {
                    m_localBorderRadiusMax = distanceFromCenter;
                    m_localBorderMaxCoordinatePixel = borderPixel;
                }
            }
            Color borderMark = new Color(1f, 0f, 0f, 1f); // Red for border pixels
            localPixels[m_localBorderMinCoordinatePixel.y * m_localTextureWidth + m_localBorderMinCoordinatePixel.x] = borderMark; // Mark min border coordinate
            localPixels[m_localBorderMaxCoordinatePixel.y * m_localTextureWidth + m_localBorderMaxCoordinatePixel.x] = borderMark; // Mark max border coordinate

            m_percentMinCompareToMaxRadius = m_localBorderRadiusMin / m_localBorderRadiusMax;
            m_isProbablyCircle = m_percentMinCompareToMaxRadius > 0.85f; // Adjust threshold as needed
            m_localTextureOfBlob.SetPixels32(localPixels);
            m_localTextureOfBlob.Apply();



            m_globalLeftPercentX = (float)m_globalLeftPixelX / (float)m_blobPixelCount.m_sourceWidth;
            m_globalRightPercentX = (float)m_globalRightPixelX / (float)m_blobPixelCount.m_sourceWidth;
            m_globalTopPercentY = (float)m_globalTopPixelY / (float)m_blobPixelCount.m_sourceHeight;
            m_globalBottomPercentY = (float)m_globalBottomPixelY / (float)m_blobPixelCount.m_sourceHeight;

            m_globalSquareCenterPercent = new Vector2(m_globalSquareCenterCoordinate.x / (float)m_blobPixelCount.m_sourceWidth, m_globalSquareCenterCoordinate.y / (float)m_blobPixelCount.m_sourceHeight);
            m_globalMassCenterPercent = new Vector2(m_globalPixelAverageCoordinate.x / (float)m_blobPixelCount.m_sourceWidth, m_globalPixelAverageCoordinate.y / (float)m_blobPixelCount.m_sourceHeight);
            m_globalHeightWidthPercent = new Vector2(m_localTextureHeight / (float)m_blobPixelCount.m_sourceHeight, m_localTextureWidth / (float)m_blobPixelCount.m_sourceWidth);



            m_centerSquareToCenterMassDirectionPixel = m_localPixelAverageCoordinate - m_localSquareCenterCoordinatePixel;
            m_centerSquareToCenterMassDirectionPercent = new Vector2(
                m_centerSquareToCenterMassDirectionPixel.x / (float)m_localTextureWidth,
                m_centerSquareToCenterMassDirectionPixel.y / (float)m_localTextureHeight
            );

            m_localTopRightCorderFromCenterPixel = new Vector2Int(m_localTextureWidth - m_localSquareCenterCoordinatePixel.x, m_localTextureHeight - m_localSquareCenterCoordinatePixel.y);
            m_localTopRightCorderFromCenterPercent = new Vector2(
                m_localTopRightCorderFromCenterPixel.x / (float)m_localTextureWidth,
                m_localTopRightCorderFromCenterPixel.y / (float)m_localTextureHeight
            );

            m_isVerticalSquare = m_localTopRightCorderFromCenterPixel.x < m_localTopRightCorderFromCenterPixel.y; // Assuming vertical if width is less than height

            m_distanceCenterToMinBorderPixel = Vector2Int.Distance(m_localSquareCenterCoordinatePixel, m_localBorderMinCoordinatePixel);
            m_distanceCenterToMaxBorderPixel = Vector2Int.Distance(m_localSquareCenterCoordinatePixel, m_localBorderMaxCoordinatePixel);
            m_distanceCenterToCorderBorderPixel = Vector2Int.Distance(m_localSquareCenterCoordinatePixel, m_localTopRightCorderFromCenterPixel);

            m_pixelHorizontalAsPercent = 1f/m_localTextureWidth;
            m_pixelVerticalAsPercent = 1f / m_localTextureHeight;

            m_globalTopRightCorner = new Vector2Int(m_globalRightPixelX, m_globalBottomPixelY);
            m_globalTopRightCornerPercent = new Vector2(
                m_globalTopRightCorner.x / (float)m_blobPixelCount.m_sourceWidth,
                m_globalTopRightCorner.y / (float)m_blobPixelCount.m_sourceHeight
            );

            Vector2Int localToGlobalDirection = m_globalSquareCenterCoordinate - m_localSquareCenterCoordinatePixel;

            m_globalBorderMinCoordinatePixel = new Vector2Int(
                m_localBorderMinCoordinatePixel.x + localToGlobalDirection.x,
                m_localBorderMinCoordinatePixel.y + localToGlobalDirection.y
            );
            m_globalBorderMaxCoordinatePixel = new Vector2Int(
                m_localBorderMaxCoordinatePixel.x + localToGlobalDirection.x,
                m_localBorderMaxCoordinatePixel.y + localToGlobalDirection.y
            );

            float distanceCenterToMinBorderPercent = Vector2Int.Distance(m_localSquareCenterCoordinatePixel, m_localBorderMinCoordinatePixel) / (float)m_localTextureWidth;
            float distanceCenterToMaxBorderPercent = Vector2Int.Distance(m_localSquareCenterCoordinatePixel, m_localBorderMaxCoordinatePixel) / (float)m_localTextureWidth;
            m_borderDeltaPercent = Mathf.Abs(distanceCenterToMaxBorderPercent-distanceCenterToMinBorderPercent);
            m_allBorderOnSameLine10Percent = (m_borderDeltaPercent < 0.10f);
            m_allBorderOnSameLine15Percent = (m_borderDeltaPercent < 0.15f);
            m_allBorderOnSameLine25Percent = (m_borderDeltaPercent < 0.25f);
            m_allBorderOnSameLine40Percent = (m_borderDeltaPercent < 0.40f);
            m_allBorderOnSameLine50Percent = (m_borderDeltaPercent < 0.50f);


        }
        public Vector2 m_globalSquareCenterPercent;
        public Vector2 m_globalMassCenterPercent;
        public Vector2 m_globalHeightWidthPercent;

        public Vector2Int m_centerSquareToCenterMassDirectionPixel;
        public Vector2 m_centerSquareToCenterMassDirectionPercent;

        public Vector2Int m_localTopRightCorderFromCenterPixel;
        public Vector2 m_localTopRightCorderFromCenterPercent;


        public Vector2Int m_globalTopRightCorner;
        public Vector2 m_globalTopRightCornerPercent;


        public float m_pixelHorizontalAsPercent;
        public float m_pixelVerticalAsPercent;
        public float m_distanceCenterToMinBorderPixel;
        public float m_distanceCenterToMaxBorderPixel;
        public float m_distanceCenterToCorderBorderPixel;

        public bool m_isVerticalSquare;
        public bool m_isAllCornersEmpty;

        /// <summary>
        /// Having holes between min and max border means that the blob is not a perfect circle or an ellipse.
        /// </summary>
        public bool m_hasHolesBetweenMinMaxBorder;
        public float m_borderDeltaPercent;
        public bool m_allBorderOnSameLine10Percent;
        public bool m_allBorderOnSameLine15Percent;
        public bool m_allBorderOnSameLine25Percent;
        public bool m_allBorderOnSameLine40Percent;
        public bool m_allBorderOnSameLine50Percent;


    }
   
    [System.Serializable]
    public class BlobPixelCount
    {

        [Range(0, 1)]
        public float m_blobPixelCountRatio;
        public int m_blobPixelCount;
        public int m_texturePixelCount;
        public int m_sourceWidth;
        public int m_sourceHeight;

        public Vector2Int m_startOfFoundBlob;
        public List<Vector2Int> m_blobPixels = new List<Vector2Int>();

        public void RefreshPixelCount(int texturePixelsCount)
        {
            m_texturePixelCount = texturePixelsCount;
            m_blobPixelCount = m_blobPixels.Count;
            if (m_texturePixelCount > 0)
            {
                m_blobPixelCountRatio = (float)m_blobPixelCount / (float)m_texturePixelCount;
            }
            else
            {
                m_blobPixelCountRatio = 0f;
            }
        }
    }

    [System.Serializable]
    public class GroupOfBlobPixelCountWithSource
    {
        public Texture2D m_texture;
        public GroupOfBlobPixelCount m_groupOfBlobs = new GroupOfBlobPixelCount();
        public GroupOfBlobPixelCountWithSource(Texture2D texture, GroupOfBlobPixelCount groupOfBlobs)
        {
            m_texture = texture;
            m_groupOfBlobs = groupOfBlobs;
        }
    }

    public class PongMono_FindBlobInTexture2D : MonoBehaviour
    {
        public Texture2D m_texture;
        public GroupOfBlobPixelCount m_groupOfBlobs = new GroupOfBlobPixelCount();


        public UnityEvent<GroupOfBlobPixelCountWithSource> m_onBlobsFoundWithSource = new UnityEvent<GroupOfBlobPixelCountWithSource>();

        public void SetTexture(Texture2D texture)
        {
            m_texture = texture;
            
        }
        public void ProcessTextureForBlobs()
        {
            Color32[] pixels = m_texture.GetPixels32();
            m_groupOfBlobs.m_blobPixelCounts.Clear();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].r > 5 || pixels[i].g > 5 || pixels[i].b > 5)
                {
                    // Assuming we want to find non-transparent pixels
                    int x = i % m_texture.width;
                    int y = i / m_texture.width;
                    BlobPixelCount blobPixelCount = new BlobPixelCount()
                    {
                        m_blobPixels = new List<Vector2Int>(),
                        m_startOfFoundBlob = new Vector2Int(x, y),
                        m_sourceWidth = m_texture.width,
                        m_sourceHeight = m_texture.height
                    };

                    EatNeighborhood(ref blobPixelCount, ref pixels);
                    m_groupOfBlobs.m_blobPixelCounts.Add(blobPixelCount);

                }
            }

            m_groupOfBlobs.m_blobPixelCounts.Sort((a, b) => b.m_blobPixelCount.CompareTo(a.m_blobPixelCount));
            GroupOfBlobPixelCountWithSource groupOfBlobPixelCountWithSource = new GroupOfBlobPixelCountWithSource(m_texture, m_groupOfBlobs);
            m_onBlobsFoundWithSource.Invoke(groupOfBlobPixelCountWithSource);

        }

        private void EatNeighborhood(ref BlobPixelCount firstBlobFound, ref Color32[] pixels)
        {
            int x , y;
            
                x = firstBlobFound.m_startOfFoundBlob.x;
                y = firstBlobFound.m_startOfFoundBlob.y;
           

            List<Vector2Int> toCheck = new List<Vector2Int>();
            toCheck.Add(new Vector2Int(x, y));

            while (toCheck.Count > 0)
            {
                Vector2Int current = toCheck[0];
                toCheck.RemoveAt(0);
                if (current.x < 0 || current.x >= m_texture.width || current.y < 0 || current.y >= m_texture.height)
                {
                    continue; 
                }
                int index = current.y * m_texture.width + current.x;
                if (pixels[index].r > 5 || pixels[index].g > 5 || pixels[index].b > 5  )
                {
                    firstBlobFound.m_blobPixels.Add(current);
                    pixels[index] = new Color32(0, 0, 0, 0); 
                    // Check neighbors
                    toCheck.Add(new Vector2Int(current.x - 1, current.y)); // Left
                    toCheck.Add(new Vector2Int(current.x + 1, current.y)); // Right
                    toCheck.Add(new Vector2Int(current.x, current.y - 1)); // Down
                    toCheck.Add(new Vector2Int(current.x, current.y + 1)); // Up
                }
            }

            firstBlobFound.RefreshPixelCount(pixels.Length);


        }


        public void SetTextureAndProcessForBlob(Texture2D texture)
        {
            SetTexture(texture);
            ProcessTextureForBlobs();
        }

       
    }

    
}
