using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialsPostprocessor : AssetPostprocessor
{
    private void OnPostprocessMaterial(Material material)
    {
        ShaderReplacer.Instance.Replace(material);
    }

    private Material OnAssignMaterialModel(Material material, Renderer renderer)
    {
        ShaderReplacer.Instance.Replace(material);
        return material;
    }
}