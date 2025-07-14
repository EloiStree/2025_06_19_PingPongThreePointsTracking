using UnityEngine;
using UnityEngine.Events;

namespace Eloi.PongTracking
{
    public class PongMono_MinMaxColorFromTextImport : MonoBehaviour
    {

        public string m_textToImport = "00 00 00 255 255 255 10";

        public STRUCT_MinMaxColorWithFlatColorThreshold m_minMaxColorFound;

        private void OnValidate()
        {
            SetTextToImportAndPush(m_textToImport);
        }


        public UnityEvent<STRUCT_MinMaxColorWithFlatColorThreshold> m_onRelayed;
        public UnityEvent<Color32> m_onRelayedMinColor;
        public UnityEvent<Color32> m_onRelayedMaxColor;
        public UnityEvent<float> m_onRelayedThreshold;

        [Header("for debug")]
        public Color32 m_minColor;
        public Color32 m_maxColor;
        public float m_threshold;

        public bool m_parseAndPushAtAwake = true;


        public void SetTextToImport(string text)
        {

            m_textToImport = text;
        }
        public void SetTextToImportAndPush(string text)
        {

            m_textToImport = text;
            ParseAndPushFromInspectorText();
        }

        [ContextMenu("Parse and push")]
        public void ParseAndPushFromInspectorText()
        {
            PongColorImportExportUtility.ImportFromText(m_textToImport, out  m_minMaxColorFound);
            m_onRelayed?.Invoke(m_minMaxColorFound);
            m_minMaxColorFound.GetMinColor(out m_minColor);
            m_minMaxColorFound.GetMaxColor(out m_maxColor);
            m_minMaxColorFound.GetFlatColorThreshold(out  m_threshold);
            m_onRelayedMinColor?.Invoke(m_minColor);
            m_onRelayedMaxColor?.Invoke(m_maxColor);
            m_onRelayedThreshold?.Invoke(m_threshold);



        }
    }
}
