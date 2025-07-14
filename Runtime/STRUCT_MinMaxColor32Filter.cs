using System;
using UnityEngine;

namespace Eloi.PongTracking
{
    [System.Serializable]
    public struct STRUCT_MinMaxColor32Filter
    {

        public byte m_minColorRed;
        public byte m_minColorGreen;
        public byte m_minColorBlue;

        public byte m_maxColorRed;
        public byte m_maxColorGreen;
        public byte m_maxColorBlue;

        public void GetMaxColor(out Color32 m_maxColor)
        {
           m_maxColor = new Color32();
            m_maxColor.r = m_maxColorRed;
            m_maxColor.g = m_maxColorGreen;
            m_maxColor.b = m_maxColorBlue;
            m_maxColor.a = 255;
        }
        public void GetMinColor(out Color32 minColor)
        {
            minColor = new Color32();
            minColor.r = m_minColorRed;
            minColor.g = m_minColorGreen;
            minColor.b = m_minColorBlue;
            minColor.a = 255;
        }

        public void ResetToDefault()
        {
            m_minColorRed = 0;
            m_minColorGreen = 0;
            m_minColorBlue = 0;
            m_maxColorRed = 255;
            m_maxColorGreen = 255;
            m_maxColorBlue = 255;
        }
    }
}