using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomShaderGUI : ShaderGUI
{

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
    }

    private static class Styles
    {
        public static GUIContent albedoText = EditorGUIUtility.TrTextContent("Albedo", "Albedo (RGB) and Transparency (A)");
        public static GUIContent metallicAoText = EditorGUIUtility.TrTextContent("Metallic AO Map", "合并的贴图");
        public static GUIContent matallicText = EditorGUIUtility.TrTextContent("Metallic", "Metallic (R) and Smoothness (A)");
        public static GUIContent occlusionText = EditorGUIUtility.TrTextContent("Occlusion", "Occlusion (G)");
        public static GUIContent metallicSwitchText = EditorGUIUtility.TrTextContent("Metallic Switch", "Metallic开关");
        public static GUIContent occlusionSwitchText = EditorGUIUtility.TrTextContent("Occlusion Switch", "Occlusion开关");
        public static GUIContent alphaCutoffText = EditorGUIUtility.TrTextContent("Alpha Cutoff", "Threshold for alpha cutoff");
        public static GUIContent alphaScaleText = EditorGUIUtility.TrTextContent("Alpha Scale", "透明度加成");
        public static GUIContent normalMapText = EditorGUIUtility.TrTextContent("Normal Map", "Normal Map");
        public static GUIContent smoothnessText = EditorGUIUtility.TrTextContent("Smoothness", "Smoothness");
        public static GUIContent smoothnessScaleText = EditorGUIUtility.TrTextContent("Smoothness", "Smoothness Scale");
        public static GUIContent cullText = EditorGUIUtility.TrTextContent("Cull", "Cull Switch");
        public static GUIContent emissionText = EditorGUIUtility.TrTextContent("Color", "Emission (RGB)");

        public static string renderingMode = "Rendering Mode";
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
    }

    MaterialEditor _materialEditor;
    MaterialProperty _blendMode = null;
    MaterialProperty _alphaCutoff = null;
    MaterialProperty _alphaScale = null;
    MaterialProperty _albedoMap = null;
    MaterialProperty _albedoColor = null;
    MaterialProperty _normalMap = null;
    MaterialProperty _smoothness = null;
    MaterialProperty _smoothnessScale = null;
    MaterialProperty _mattlicSwitch = null;
    MaterialProperty _matallicSwitch = null;
    MaterialProperty _occlusionSwitch = null;
    MaterialProperty _occlusionStrength = null;
    MaterialProperty _metallic = null;
    MaterialProperty _cull = null;
    MaterialProperty _metallicMap = null;
    MaterialProperty _emissionMap = null;
    MaterialProperty _emissionColor = null;

    private void _drawEmission()
    {
        EditorGUI.BeginChangeCheck();
        // Emission for GI?
        if (_materialEditor.EmissionEnabledProperty())
        {
            bool hadEmissionTexture = _emissionMap.textureValue != null;

            // Texture and HDR color controls
            _materialEditor.TexturePropertyWithHDRColor(Styles.emissionText, _emissionMap, _emissionColor, false);

            // If texture was assigned and color was black set color to white
            float brightness = _emissionColor.colorValue.maxColorComponent;
            if (_emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                _emissionColor.colorValue = Color.white;

            // change the GI flag and fix it up with emissive as black if necessary
            _materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
        }

        if (EditorGUI.EndChangeCheck())
        {
            var material = _materialEditor.target as Material;
            MaterialEditor.FixupEmissiveFlag(material);
            bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
            SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);
        }
    }

    private void _drawCullSwitch()
    {
        _materialEditor.ShaderProperty(_cull, Styles.cullText.text);
    }

    private void _drawNormal()
    {
        _materialEditor.TexturePropertySingleLine(Styles.normalMapText, _normalMap);
    }

    private void _drawMettalicAO(MaterialProperty[] props)
    {
        if (_metallicMap.textureValue == null)
        {
            _materialEditor.TexturePropertySingleLine(Styles.metallicAoText, _metallicMap);
            _materialEditor.ShaderProperty(_metallic, Styles.matallicText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel);
            _materialEditor.ShaderProperty(_smoothness, Styles.smoothnessText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel);
        }
        else
        {
            _materialEditor.TexturePropertySingleLine(Styles.metallicAoText, _metallicMap, _matallicSwitch, _occlusionSwitch);
            if (_matallicSwitch.floatValue > 0)
            {
                _materialEditor.ShaderProperty(_smoothnessScale, Styles.smoothnessScaleText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
            }

            if (_occlusionSwitch.floatValue > 0)
            {
                _materialEditor.ShaderProperty(_occlusionStrength, Styles.occlusionText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
            }
        }
    }

    private void _drawAlbedo(MaterialProperty[] props)
    {
        var material = _materialEditor.target as Material;

        _materialEditor.TexturePropertySingleLine(Styles.albedoText, _albedoMap, _albedoColor);
    }

    private void _drawAlphaArea(MaterialProperty[] props)
    {
        var material = _materialEditor.target as Material;

        if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
        {
            _materialEditor.ShaderProperty(_alphaCutoff, Styles.alphaCutoffText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
        }
        else
        {
            _materialEditor.ShaderProperty(_alphaScale, Styles.alphaScaleText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
        }
    }

    private void _setMattalicAoStatus(MaterialProperty[] props)
    {
        var propterty = ShaderGUI.FindProperty("_MatallicSwitch", props);
        bool mattlicEnable = propterty.floatValue > 0 && _metallicMap.textureValue != null;
        propterty = ShaderGUI.FindProperty("_OcclusionSwitch", props);
        bool occlusionEnable = propterty.floatValue > 0 && _metallicMap.textureValue != null;


        Material material = _materialEditor.target as Material;
        if (mattlicEnable)
            material.EnableKeyword("_METALLICGLOSSMAP");
        else
            material.DisableKeyword("_METALLICGLOSSMAP");

        if (occlusionEnable)
            material.EnableKeyword("_OCCLUSION_ENABLE");
        else
            material.DisableKeyword("_OCCLUSION_ENABLE");
    }

    private void _drawBlendMode()
    {
        EditorGUI.showMixedValue = _blendMode.hasMixedValue;
        var mode = (BlendMode)_blendMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
        if (EditorGUI.EndChangeCheck())
        {
            _materialEditor.RegisterPropertyChangeUndo("Rendering Mode");
            _blendMode.floatValue = (float)mode;
            SetupMaterialWithBlendMode(_materialEditor.target as Material, mode);
        }

        EditorGUI.showMixedValue = false;
    }

    private void _findProperty(MaterialProperty[] props)
    {
        _blendMode = FindProperty("_Mode", props);
        _alphaCutoff = FindProperty("_Cutoff", props);
        _alphaScale = FindProperty("_AlphaScale", props);
        _albedoMap = FindProperty("_MainTex", props);
        _albedoColor = FindProperty("_Color", props);
        _normalMap = FindProperty("_BumpMap", props);
        _smoothness = FindProperty("_Glossiness", props);
        _smoothnessScale = FindProperty("_GlossMapScale", props);
        _matallicSwitch = FindProperty("_MatallicSwitch", props);
        _occlusionSwitch = FindProperty("_OcclusionSwitch", props);
        _occlusionStrength = FindProperty("_OcclusionStrength", props);
        _metallic = FindProperty("_Metallic", props);
        _cull = FindProperty("_Cull", props);
        _metallicMap = FindProperty("_MetallicGlossMap", props, false);
        _emissionMap = FindProperty("_EmissionMap", props);
        _emissionColor = FindProperty("_EmissionColor", props);
    }

    public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }

    override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {

        EditorGUI.BeginChangeCheck();
        _materialEditor = materialEditor;

        _findProperty(props);

        _drawBlendMode();
        _drawCullSwitch();
        _drawAlbedo(props);
        _drawAlphaArea(props);
        _drawNormal();
        _drawMettalicAO(props);
        _drawEmission();
        _materialEditor.TextureScaleOffsetProperty(_albedoMap);
        _setMattalicAoStatus(props);

        // base.OnGUI(materialEditor, props);
    }

    static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }

}

