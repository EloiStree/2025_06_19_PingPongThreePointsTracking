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
        public Texture2D m_texture;
        public int m_globalLeftX;
        public int m_globalRightX;
        public int m_globalTopY;
        public int m_globalBottomY;

        public int m_width;
        public int m_height;
        public Vector2Int m_localSquareCenterCoordinate;
        public Vector2Int m_globalSquareCenterCoordinate;
        public Vector2Int m_globalPixelAverageCoordinate;
        public Vector2Int m_localPixelAverageCoordinate;

        public List<Vector2Int> m_emptyAroundFromBorder;
        public List<Vector2Int> m_borderPixels = new List<Vector2Int>();

        public float m_localBorderRadiusMin;
        public float m_localBorderRadiusMax;

        public Vector2Int m_localBorderMinCoordinate;
        public Vector2Int m_localBorderMaxCoordinate;

        [Range(0,1)]
        public float m_percentMinCompareToMaxRadius;
        public bool m_isProbablyCircle;

        public void SetWithBlobPixelCount(BlobPixelCount blobPixelCount)
        {
            m_blobPixelCount = blobPixelCount;
            m_globalLeftX = int.MaxValue;
            m_globalRightX = int.MinValue;
            m_globalTopY = int.MaxValue;
            m_globalBottomY = int.MinValue;
            foreach (Vector2Int pixel in blobPixelCount.m_blobPixels)
            {
                if (pixel.x < m_globalLeftX) m_globalLeftX = pixel.x;
                if (pixel.x > m_globalRightX) m_globalRightX = pixel.x;
                if (pixel.y < m_globalTopY) m_globalTopY = pixel.y;
                if (pixel.y > m_globalBottomY) m_globalBottomY = pixel.y;
            }
            m_width = m_globalRightX - m_globalLeftX + 1;
            m_height = m_globalBottomY - m_globalTopY + 1;
            m_localSquareCenterCoordinate = new Vector2Int( m_width / 2,  m_height / 2);
            m_globalSquareCenterCoordinate = new Vector2Int(m_globalLeftX + m_localSquareCenterCoordinate.x, m_globalTopY + m_localSquareCenterCoordinate.y);

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

            m_localPixelAverageCoordinate = m_localSquareCenterCoordinate + centreOfMassDirectionFromCenter / blobPixelCount.m_blobPixels.Count;

            m_texture = new Texture2D(m_width, m_height);
            m_texture.filterMode = FilterMode.Point;
            m_texture.wrapMode = TextureWrapMode.Clamp;
            Color32[] localPixels = new Color32[m_width * m_height];
            Color32 borderColor = new Color32(255, 0, 0, 255); // Red for the border
            for (int i = 0; i < localPixels.Length; i++)
            {
                localPixels[i] = new Color32(0, 0, 0, 0); // Transparent
               

            }
            foreach (Vector2Int pixel in blobPixelCount.m_blobPixels)
            {
                int localX = pixel.x - m_globalLeftX;
                int localY = pixel.y - m_globalTopY;
                if (localX >= 0 && localX < m_width && localY >= 0 && localY < m_height)
                {
                    localPixels[localY * m_width + localX] = new Color32(255, 255, 255, 255); // White for blob pixels
                }
            }
          

            Color32 squareCenter = new Color32(0, 255, 255, 255);
            Color32 squareMassCenter = new Color32(0, 255, 0, 255);
            localPixels[m_localSquareCenterCoordinate.y * m_width + m_localSquareCenterCoordinate.x] = squareCenter; // Cyan for local center
            localPixels[m_localPixelAverageCoordinate.y * m_width + m_localPixelAverageCoordinate.x] = squareMassCenter; // Green for local average center
           


            List<Vector2Int> borderStartPoint = new List<Vector2Int>();
            for (int y = 0; y < m_height; y++)
            {
                borderStartPoint.Add(new Vector2Int(0, y)); // Left border
                borderStartPoint.Add(new Vector2Int(m_width - 1, y)); // Right border
            }
            for (int x = 0; x < m_width; x++)
            {
                borderStartPoint.Add(new Vector2Int(x, 0)); // Top border
                borderStartPoint.Add(new Vector2Int(x, m_height - 1)); // Bottom border
            }
            m_emptyAroundFromBorder = new List<Vector2Int>();
            List<Vector2Int> toExplore = new List<Vector2Int>(borderStartPoint);
            while (toExplore.Count > 0)
            {
                Vector2Int current = toExplore[0];
                toExplore.RemoveAt(0);
                if (current.x < 0 || current.x >= m_width || current.y < 0 || current.y >= m_height)
                {
                    continue; // Out of bounds
                }
                int index = current.y * m_width + current.x;
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
                            if (neighborX >= 0 && neighborX < m_width && neighborY >= 0 && neighborY < m_height)
                            {
                                int neighborIndex = neighborY * m_width + neighborX;
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
                Vector2Int direction = borderPixel - m_localSquareCenterCoordinate;
                float distanceFromCenter = direction.magnitude;
                if (distanceFromCenter < m_localBorderRadiusMin)
                {
                    m_localBorderRadiusMin = distanceFromCenter;
                    m_localBorderMinCoordinate = borderPixel;
                }
                if (distanceFromCenter > m_localBorderRadiusMax)
                {
                    m_localBorderRadiusMax = distanceFromCenter;
                    m_localBorderMaxCoordinate = borderPixel;
                }
            }
            Color borderMark = new Color(1f, 0f, 0f, 1f); // Red for border pixels
            localPixels[m_localBorderMinCoordinate.y * m_width + m_localBorderMinCoordinate.x] = borderMark; // Mark min border coordinate
            localPixels[m_localBorderMaxCoordinate.y * m_width + m_localBorderMaxCoordinate.x] = borderMark; // Mark max border coordinate

            m_percentMinCompareToMaxRadius = m_localBorderRadiusMin / m_localBorderRadiusMax;
            m_isProbablyCircle = m_percentMinCompareToMaxRadius > 0.85f; // Adjust threshold as needed
            m_texture.SetPixels32(localPixels);
            m_texture.Apply();

        }

      
    }
   
    [System.Serializable]
    public class BlobPixelCount
    {

        [Range(0, 1)]
        public float m_blobPixelCountRatio;
        public int m_blobPixelCount;
        public int m_texturePixelCount;

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
                        m_startOfFoundBlob = new Vector2Int(x, y)
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
