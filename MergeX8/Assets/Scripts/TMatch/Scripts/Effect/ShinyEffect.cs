// **********************************************
// Copyright(c) 2020 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-03-11 3:15 PM
// Ver : 1.0.0
// Description : ShinyMaterialPropertyUpdater.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ShinyEffect : MonoBehaviour, IMaterialModifier
#if UNITY_EDITOR
    , ISerializationCallbackReceiver
#endif
{
    [SerializeField] [Range(0, 1)] float m_Location = 0;
    [SerializeField] [Range(0, 1)] float m_Width = 0.25f;
    [SerializeField] [Range(0.01f, 1)] float m_Softness = 1f;
    [SerializeField] [Range(0, 1)] float m_Brightness = 1f;
    [SerializeField] [Range(0, 360)] float m_Rotation;
    [SerializeField] [Range(0, 1)] float m_Highlight = 1;
    [SerializeField] [Range(0, 3)] float m_HighlightThreshold = 0;
    [SerializeField] [Range(0, 1)] float m_HighlightSoftFactor = 0;
    [SerializeField] Color m_ShinyColor = Color.white;

    Material effectMaterial;

    private Material modifyMaterial;

    private Image image;
    private SpriteRenderer spriteRenderer;

    private Vector4 spriteRect;
    private Sprite _sprite;

    private bool waitRef = false;
    private void OnDidApplyAnimationProperties()
    {
        UpdateMaterial();
    }
    
    private void UpdateMaterial()
    {
        if (!Application.isPlaying)
        {
            image = GetComponent<Image>();
            spriteRenderer = GetComponent<SpriteRenderer>();
           
            if (spriteRenderer != null)
            {
                if (Application.isPlaying)
                {
                    effectMaterial = spriteRenderer.material;
                }
                else
                {
                    effectMaterial = spriteRenderer.sharedMaterial;
                }
            }
            else if(image != null)
            {
                effectMaterial = image.material;
            }
        }
        
        if (modifyMaterial != null && modifyMaterial.shader.name.Contains("Shiny"))
        {
            UpdateMaterialProperty(modifyMaterial);
        }

        if (!Application.isPlaying || modifyMaterial == null)
        {
            if (effectMaterial != null && effectMaterial.shader.name.Contains("Shiny"))
            {
                UpdateMaterialProperty(effectMaterial);
            }
        }
    }

#if UNITY_EDITOR
    public void OnBeforeSerialize()
    {
       // UpdateMaterial();
    }
    
    public void OnAfterDeserialize()
    {
    }
#endif

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        Check();
        baseMaterial.SetVector("_SpriteText_Rect", spriteRect);

        UpdateMaterialProperty(baseMaterial);
    
        modifyMaterial = baseMaterial;
        
        return baseMaterial;
    }

    public void UpdateMaterialProperty(Material material)
    {
        material.SetFloat("_ShinyLocation", m_Location);
        material.SetFloat("_ShinyWidth", m_Width);
        material.SetFloat("_ShinyRotation", m_Rotation);
        material.SetFloat("_ShinyHighlight", m_Highlight);
        material.SetFloat("_ShinySoftness", m_Softness);
        material.SetFloat("_ShinyBrightness", m_Brightness);
        material.SetColor("_ShinyColor", m_ShinyColor);
        material.SetFloat("_HighlightThreshold", m_HighlightThreshold);
        material.SetFloat("_HighlightSoftFactor", m_HighlightSoftFactor);
    }
    
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (spriteRenderer != null && spriteRenderer.sprite != _sprite || image != null && image.sprite != _sprite
        || waitRef)
        {
            Init();
        }
    }

    private void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        _sprite = null;

        if (spriteRenderer != null)
        {
            _sprite = spriteRenderer.sprite;
            if (Application.isPlaying)
            {
                effectMaterial = new Material(spriteRenderer.sharedMaterial);
                spriteRenderer.material = effectMaterial;
            }
            else
            {
                effectMaterial = spriteRenderer.sharedMaterial;
            }
        }

        image = GetComponent<Image>();

        if (image != null)
        {
            _sprite = image.sprite;
            if (Application.isPlaying)
            {
                effectMaterial = new Material(image.material);
                image.material = effectMaterial;
            }
            else
            {
                effectMaterial = image.material;
            }
        }

        if (_sprite == null || effectMaterial == null)
            return;

        if (!effectMaterial.shader.name.Contains("Shiny"))
        {
            effectMaterial = null;
            return;
        }

        var uv = _sprite.uv;
        Vector2 uvMin = Vector2.one;
        Vector2 uvMax = Vector2.zero;
       
        for (var i = 0; i < uv.Length; i++)
        {
            if (uv[i].x < uvMin.x)
            {
                uvMin.x = uv[i].x;
            }

            if (uv[i].x > uvMax.x)
            {
                uvMax.x = uv[i].x;
            }

            if (uv[i].y < uvMin.y)
            {
                uvMin.y = uv[i].y;
            }

            if (uv[i].y > uvMax.y)
            {
                uvMax.y = uv[i].y;
            }
        }
        spriteRect = new Vector4(uvMin.x, uvMin.y, uvMax.x - uvMin.x, uvMax.y - uvMin.y);
        Check();
        effectMaterial.SetVector("_SpriteText_Rect", spriteRect);

        UpdateMaterialProperty(effectMaterial);
    }

    private int checkTimes = 0;
    private const int TIMES = 120;
    
    void Check()
    {
        waitRef = false;
        
        checkTimes++;
        if(checkTimes >= TIMES)
            return;
        
        if (spriteRect.x <= 0 && spriteRect.y <= 0)
            waitRef = true;

        if (spriteRect.z <= 0)
        {
            spriteRect.z = 1;
            waitRef = true;
        }
        if (spriteRect.w <= 0)
        {
            spriteRect.w = 1;
            waitRef = true;
        }
    }
}