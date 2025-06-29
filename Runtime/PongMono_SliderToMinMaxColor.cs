
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{



    public class PongMono_SliderToMinMaxColor : MonoBehaviour
    {


        [Range(0, 255)] public byte m_minRed255;
        [Range(0, 255)] public byte m_minGreen255;
        [Range(0, 255)] public byte m_minBlue255;
        [Range(0, 255)] public byte m_maxRed255;
        [Range(0, 255)] public byte m_maxGreen255;
        [Range(0, 255)] public byte m_maxBlue255;

        public string m_exportAsDigit;
        public Color32 m_min;
        public Color32 m_max;

        public Color32[] m_sampleColors = new Color32[10];


        public UnityEvent<Color32> m_onChangedMinColor;
        public UnityEvent<Color32> m_onChangedMaxColor;
        public UnityEvent<Color32[]> m_onChangedSampleColor;



        public bool m_refreshAtAwake = true;
        public void Awake()
        {
            if (m_refreshAtAwake)
                Refresh();
        }
        private void OnValidate()
        {
            Refresh();
        }

        public void SetMinRed255(int red) { m_minRed255 = (byte)red; Refresh(); }
        public void SetMinGreen255(int green) { m_minGreen255 = (byte)green; Refresh(); }
        public void SetMinBlue255(int blue) { m_minBlue255 = (byte)blue; Refresh(); }
        public void SetMaxRed255(int red) { m_maxRed255 = (byte)red; Refresh(); }
        public void SetMaxGreen255(int green) { m_maxGreen255 = (byte)green; Refresh(); }
        public void SetMaxBlue255(int blue) { m_maxBlue255 = (byte)blue; Refresh(); }
        private void Refresh()
        {

            if (m_maxRed255 < m_minRed255) m_maxRed255 = m_minRed255;
            if (m_maxGreen255 < m_minGreen255) m_maxGreen255 = m_minGreen255;
            if (m_maxBlue255 < m_minBlue255) m_maxBlue255 = m_minBlue255;

            m_min.r = m_minRed255;
            m_min.g = m_minGreen255;
            m_min.b = m_minBlue255;
            m_max.r = m_maxRed255;
            m_max.g = m_maxGreen255;
            m_max.b = m_maxBlue255;
            m_min.a = 0;
            m_max.a = 255;


            for (int i = 0; i < m_sampleColors.Length; i++)
            {
                m_sampleColors[i].r = (byte)UnityEngine.Random.Range(m_min.r, m_max.r);
                m_sampleColors[i].g = (byte)UnityEngine.Random.Range(m_min.g, m_max.g);
                m_sampleColors[i].b = (byte)UnityEngine.Random.Range(m_min.b, m_max.b);
            }

            PongColorImportExportUtility.BuildExportMinMaxColor32AsDigit(out m_exportAsDigit,
                m_minRed255,
                m_minGreen255,
                m_minBlue255,
                m_maxRed255,
                m_maxGreen255,
                m_maxBlue255
                );

            m_onChangedMaxColor?.Invoke(m_max);
            m_onChangedMinColor?.Invoke(m_min);
            m_onChangedSampleColor?.Invoke(m_sampleColors);

        }
    }
}
