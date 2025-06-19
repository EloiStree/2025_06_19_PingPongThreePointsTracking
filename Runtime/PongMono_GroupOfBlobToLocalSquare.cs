// Copyright (c) Meta Platforms, Inc. and affiliates.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace Eloi.PongTracking
{
    [System.Serializable]
    public class LocalBlobsTextureWithSource
    {

        public GroupOfBlobPixelCountWithSource m_groupOfBlobs;
        public List<LocalBlobTexture> m_localBlobTextures = new List<LocalBlobTexture>();

    }

}