using System;
using UnityEngine;

[ExecuteInEditMode]
public class KeepProportionScaleByRectTransformTarget:MonoBehaviour
{
    public KeepProportionScaleByRectTransform Controller;

    private void Awake()
    {
    }

    public void OnRectTransformDimensionsChange()
    {
        if (Controller)
            Controller.OnRectTransformDimensionsChange();
        else
            DestroyImmediate(this);
    }
}