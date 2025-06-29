using System;

namespace Eloi.PongTracking
{
    /// <summary>
    /// I am struct container that hold a threshold value for flat color (black gray white)
    /// </summary>
    [System.Serializable]
    public struct STRUCT_FlatColorDelta
    {
        public float m_flatColorDeltaPercent;

        public void ResetToDefault()
        {
            m_flatColorDeltaPercent = 0f;
        }

        public void GetFlatColorThreshold(out float threshold)
        {
            threshold = m_flatColorDeltaPercent;
        }
    }
}
