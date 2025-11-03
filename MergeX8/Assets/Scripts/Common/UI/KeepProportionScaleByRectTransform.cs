using System;
using UnityEngine;

[ExecuteInEditMode]
public class KeepProportionScaleByRectTransform:MonoBehaviour
{
    private RectTransform BindingRect;
    public RectTransform ControllRect;
    public float MinScale = 0;
    public float MaxScale = 1;
    public bool IsEnable => BindingRect != null;

    private void Awake()
    {
        if (!transform)
            return;
        BindingRect = transform.GetComponent<RectTransform>();
        if (BindingRect == null)
            enabled = false;
        OnRectTransformDimensionsChange();
        InitTarget();
    }

    public void InitTarget()
    {
        if (ControllRect)
        {
            var targets = ControllRect.gameObject.GetComponentsInChildren<KeepProportionScaleByRectTransformTarget>();
            var existTarget = false;
            foreach (var target in targets)
            {
                if (target && target.Controller == this)
                {
                    if (!existTarget)
                    {
                        existTarget = true;   
                    }
                    else
                    {
                        DestroyImmediate(target);
                    }
                }
            }

            if (!existTarget)
            {
                var target = ControllRect.gameObject.AddComponent<KeepProportionScaleByRectTransformTarget>();
                target.Controller = this;
            }
        }
    }

    private void OnEnable()
    {
        if (!transform)
            return;
        BindingRect = transform.GetComponent<RectTransform>();
        if (BindingRect == null)
            enabled = false;
        OnRectTransformDimensionsChange();
        InitTarget();
    }

    public void OnRectTransformDimensionsChange()
    {
        if (!BindingRect)
            return;
        if (!ControllRect)
            return;
        if (!enabled)
            return;
        if (!gameObject.activeSelf)
            return;
        var width = BindingRect.rect.width;
        var height = BindingRect.rect.height;
        var targetWidth = ControllRect.rect.width;
        var targetHeight = ControllRect.rect.height;
        var scaleX = width / targetWidth;
        var scaleY = height / targetHeight;
        var minScale = Math.Min(scaleX, scaleY);
        if (minScale > MaxScale)
            minScale = MaxScale;
        if (minScale < MinScale)
            minScale = MinScale;
        ControllRect.localScale = new Vector3(minScale, minScale, 1);
    }
}