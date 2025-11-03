/****************************************************
    文件：FixedColor.cs
    作者：Cys
    邮箱: yongsheng.chen@dragonplus.com
    日期： 2021-11-04-16:39:07
    功能：.... 调整 饱和度 和亮度 对比度
*****************************************************/
using UnityEngine;
using UnityEngine.UI;

public class FixedColor : MonoBehaviour
{
    private float saturation = 1; // 饱和度
    private float brightness = 1; // 亮度
    private float contrast = 1; // 对比度
    private float alpha = 1; // 透明度

    public float Saturation
    {
        get { return saturation; }
        set { saturation = value; }
    }

    public float Brightness
    {
        get { return brightness; }
        set { brightness = value; }
    }

    public float Contrast
    {
        get { return contrast; }
        set { contrast = value; }
    }

    private int m_ShaderID_Saturation;
    private int m_ShaderID_Brightness;
    private int m_ShaderID_Contrast;
    private int m_ShaderID_Alpha;
    private Material m_Material;
    private Graphic m_Graphic;

    private void Awake()
    {
        m_ShaderID_Saturation = Shader.PropertyToID("_Saturation");
        m_ShaderID_Brightness = Shader.PropertyToID("_Brightness");
        m_ShaderID_Contrast = Shader.PropertyToID("_Contrast");
        m_ShaderID_Alpha = Shader.PropertyToID("_Alpha");
        m_Material = new Material(Shader.Find("Color/FixColor"));
        m_Material.name = "FixColor";
        m_Graphic = this.GetComponent<Graphic>();
    }

    private void Start()
    {
        if (m_Graphic != null && m_Material != null)
        {
            m_Graphic.material = m_Material;
        }
    }

    public void SetWeblockSaturation() // 蛛网 饱和度
    {
        Saturation = 0.6f;
        Brightness = 0.85f;
        alpha = 0.7f;
        Apply();
    }

    public void SetLockBoxSaturation() // 盒子的饱和度
    {
        Saturation = 0.6f;
        Brightness = 0.85f;
        Apply();
    }

    public void RecoverNormal() // 恢复正常
    {
        Saturation = 1f;
        Brightness = 1f;
        alpha = 1f;
        contrast = 1f;
        Apply();
    }

    private void Apply()
    {
        if (m_Material != null && m_Graphic != null)
        {
            m_Material.SetFloat(m_ShaderID_Saturation, Saturation);
            m_Material.SetFloat(m_ShaderID_Brightness, Brightness);
            m_Material.SetFloat(m_ShaderID_Contrast, Contrast);
            m_Material.SetFloat(m_ShaderID_Alpha, alpha);
        }
    }

    private void OnDestroy()
    {
        if (m_Graphic != null)
        {
            if (m_Graphic.material == m_Material)
            {
                m_Graphic.material = null;
            }
        }

        if (m_Material != null)
        {
            if (Application.isPlaying)
                GameObject.Destroy(m_Material);
            else
                GameObject.DestroyImmediate(m_Material);
            m_Material = null;
        }
    }
}