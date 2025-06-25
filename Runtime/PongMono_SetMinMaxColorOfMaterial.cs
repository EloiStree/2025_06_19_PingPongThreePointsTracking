using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;

public class PongMono_SetMinMaxColorOfMaterial : MonoBehaviour
{

    public Color32 m_minColorRange;
    public Color32 m_maxColorRange;
    public Material m_materialToAffect;

    public string m_minColorName = "_ColorMin";
    public string m_maxColorName = "_ColorMax";



    public void SetMaterialToAffect(Material material)
    { 
    
        m_materialToAffect = material;
    }

    private void OnValidate()
    {
        SetMinMaxColorRange(m_minColorRange, m_maxColorRange);
    }

    public void SetMinMaxColorRange(Color32 minColor, Color32 maxColor) {

        SetMinColorRange(minColor);
        SetMaxColorRange(maxColor);
    }
    public void SetMinColorRange(Color32 minColorRange)
    {

        m_minColorRange = minColorRange;
        if (m_materialToAffect != null)
            m_materialToAffect.SetColor(m_minColorName, m_minColorRange);
    }
    public void SetMaxColorRange(Color32 maxColorRange)
    {

        m_maxColorRange = maxColorRange;
        if (m_materialToAffect != null)
            m_materialToAffect.SetColor(m_maxColorName, m_maxColorRange);
    }

    public void SetMinMaxColorRange(Color minColor, Color maxColor)
    {

        SetMinColorRange(minColor);
        SetMaxColorRange(maxColor);
    }
    public void SetMinColorRange(Color minColorRange)
    {
        Color32 c = minColorRange            ;
        SetMinColorRange(c);

    }
    public void SetMaxColorRange(Color maxColorRange)
    {
        Color32 c = maxColorRange            ;
        SetMaxColorRange(c);
    

    }
}
