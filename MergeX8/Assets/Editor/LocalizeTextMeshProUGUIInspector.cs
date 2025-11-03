using System;
using System.Linq;
using DragonPlus;
using UnityEditor;
using UnityEngine;
using TMPro;

[CustomEditor(typeof(LocalizeTextMeshProUGUI))]
public sealed class LocalizeTextMeshProUGUIInspector : Editor
{
    //private TMPSettings m_settings;
    private string mm="";
    private string tt="";
    private string last_tt="";
    LocalizeTextMeshProUGUI tmpText;
    TextMeshProUGUI meshProGUI;
    private void OnEnable()
    {
        //m_settings=new TMPSettings();
        //m_settings = TMPSettingsEditorUtils.GetSettings();
        //var localizeText = target as LocalizeTextMeshProUGUI;
        //TMPSettingsEditorUtils.Apply(m_settings, localizeText);

        tmpText = target as LocalizeTextMeshProUGUI;
        meshProGUI=tmpText.GetComponent<TextMeshProUGUI>();
        mm=tmpText.GetMaterialString();
        last_tt=tt=tmpText.GetTermString();
    }

    public override void OnInspectorGUI()
    {
        if (tmpText == null) return;
        if(meshProGUI==null)return;

        #region 显示材质后缀
        tt=EditorGUILayout.TextField("Term", last_tt);
        EditorGUILayout.TextField("Material", mm);
        if(last_tt!=tt){
            tmpText.SetTermStringValue(tt);
            last_tt=tt;
        }
        //GUI.enabled = true;
        #endregion



        #region 刷新按钮
        if (GUILayout.Button("设置材质"))
        {
            Debug.Log(meshProGUI.fontSharedMaterial.name);
            var mstr=meshProGUI.fontSharedMaterial.name.Replace("LocaleFont_En SDF",string.Empty).TrimStart().TrimEnd();
            Debug.Log(mstr);
            tmpText.SetMaterialStringValue(mstr);
            mm=tmpText.GetMaterialString();

        }
        #endregion
    }
}
