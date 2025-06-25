
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PongMono_ColorCollectionToMinMax : MonoBehaviour
{

    public System.Collections.Generic.List<Color32> m_colorCollection = new List<Color32> ();
    public Color32 m_minColorRange;
    public Color32 m_maxColorRange;

    public Color32[] m_sampleColors = new Color32 [10];

    public UnityEvent<Color32> m_onChangedMinColor;
    public UnityEvent<Color32> m_onChangedMaxColor;
    public UnityEvent<Color32[]> m_onChangedSampleColor;


    public bool m_refreshAtAwake = false;
    public void Awake()
    {
        if (m_refreshAtAwake)
            Refresh();
    }
    [ContextMenu("Add random color")]
    public void AddRandomColor() {
        Color32 color = new Color32()
            ;
        color.r = (byte)(Random.value * 255);
        color.g = (byte)(Random.value * 255);
        color.b = (byte)(Random.value * 255);
        AddColor(color);
    }

    public void AddColor(Color32 color) { 
    
        m_colorCollection.Add (color);
        Refresh();
    }

    [ContextMenu("Clear colors")]
    public void ClearColors() { 
    
        m_colorCollection.Clear ();
        Refresh();
    }

    private void OnValidate()
    {
        Refresh();
    }
    private void Refresh()
    {

        m_minColorRange = Color.white;
        m_maxColorRange = Color.black;

        for (int i = 0; i < m_colorCollection.Count; i++)
        {

            if (m_colorCollection[i].r <= m_minColorRange.r) m_minColorRange.r = m_colorCollection[i].r;
            if (m_colorCollection[i].g <= m_minColorRange.g) m_minColorRange.g = m_colorCollection[i].g;
            if (m_colorCollection[i].b <= m_minColorRange.b) m_minColorRange.b = m_colorCollection[i].b;
            if (m_colorCollection[i].r >= m_maxColorRange.r) m_maxColorRange.r = m_colorCollection[i].r;
            if (m_colorCollection[i].g >= m_maxColorRange.g) m_maxColorRange.g = m_colorCollection[i].g;
            if (m_colorCollection[i].b >= m_maxColorRange.b) m_maxColorRange.b = m_colorCollection[i].b;

        }

        for (int i = 0; i < m_sampleColors.Length; i++)
        {
            m_sampleColors[i].r = (byte)UnityEngine.Random.Range(m_minColorRange.r, m_maxColorRange.r);
            m_sampleColors[i].g = (byte)UnityEngine.Random.Range(m_minColorRange.g, m_maxColorRange.g);
            m_sampleColors[i].b = (byte)UnityEngine.Random.Range(m_minColorRange.b, m_maxColorRange.b);
        }

        m_onChangedMinColor?.Invoke(m_minColorRange);
        m_onChangedMaxColor?.Invoke(m_maxColorRange);
        m_onChangedSampleColor?.Invoke(m_sampleColors);


    }
}
