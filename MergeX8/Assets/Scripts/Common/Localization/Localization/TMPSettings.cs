using System;
using TMPro;
using UnityEngine;

[CreateAssetMenu(order = 9999)]
public class TMPSettings : ScriptableObject
{
    [SerializeField] private TMPSettingParam[] m_list = null;

    public TMPSettingParam[] List
    {
        get { return m_list; }

        set { m_list = value; }
    }
}

[Serializable]
public sealed class TMPSettingParam
{
    [SerializeField] private bool m_isLock = false; // 是否禁止更改
    [SerializeField] private string m_name = null; // 名字
    [SerializeField] private string m_materialSuffix = null; // 材质球后缀
    [SerializeField] private float m_characterSpacing = 0f; // 字符间距
    [SerializeField] private float m_wordSpacing = 0f; // 词间距
    [SerializeField] private float m_lineSpacing = 0f; // 行间距
    [SerializeField] private float m_paragraphSpacing = 0f; // 段间距
    [SerializeField] private TMP_FontAsset m_fontAsset = null;
    [SerializeField] private Material m_material = null;


    public bool IsLock
    {
        get { return m_isLock; }
        set { m_isLock = value; }
    }

    public string Name => m_name;
    public string MaterialSuffix => m_materialSuffix;
    public float CharacterSpacing => m_characterSpacing;
    public float WordSpacing => m_wordSpacing;
    public float LineSpacing => m_lineSpacing;
    public float ParagraphSpacing => m_paragraphSpacing;
    public TMP_FontAsset FontAsset => m_fontAsset;
    public Material Material => m_material;
}