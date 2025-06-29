
using System;
using UnityEngine;

namespace Eloi.PongTracking
{
    [System.Serializable]
    public struct STRUCT_MinMaxColorWithFlatColorThreshold
    {

        public STRUCT_MinMaxColor32Filter m_filter;
        public STRUCT_FlatColorDelta m_threshold;


        public void ResetToDefault()
        {
            m_threshold.ResetToDefault();
            m_filter.ResetToDefault();
        }

        public void GetFlatColorThreshold(out float threshold)
        {
            m_threshold.GetFlatColorThreshold(out threshold);
        }

        public void GetMaxColor(out Color32 maxColor)
        {
            m_filter.GetMaxColor(out maxColor);
        }

        public void GetMinColor(out Color32 minColor)
        {
            m_filter.GetMinColor(out minColor);
        }
    }
}