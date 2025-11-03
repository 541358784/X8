using Screw;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Screw
{
    public enum ColorReceiverType
    {
        Panel = 0,
        PanelShadow = 1,
        Module = 2
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class ColorProvider : MonoBehaviour
    {
        public ColorType colorType;

        private Material material;

        private SpriteRenderer spriteRenderer;

        private ColorReceiverType receiverType;

        private ScrewGameContext context;
        private int layer = 0;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            if (name.Contains("Screw"))
            {
                OnColorChange(colorType);
            }
        }

        public void OnColorChange(ColorType color)
        {
            var topSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Export/Screw/Textures/Screw/{color}_Screw.png");

            transform.Find("ScrewBody/ScrewTop").GetComponent<SpriteRenderer>().sprite = topSprite;
        }
#endif
        

        public void SetColor(ColorType inColorType, ColorReceiverType inReceiverType, ScrewGameContext inContext, int inLayer = 0)
        {
            colorType = inColorType;
            receiverType = inReceiverType;
            context = inContext;
            layer = inLayer;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            material = spriteRenderer.sharedMaterial;

            if (material == null)
                return;
            Material newMaterial = new Material(material);

            spriteRenderer.sharedMaterial = newMaterial;

            UpdateMaterials();
        }

        private void UpdateMaterials()
        {
            if (material != null)
            {
                var color = Color.black;
                switch (receiverType)
                {
                    case ColorReceiverType.Panel:
                        color = context.ColorDic[colorType].panel;
                        break;
                    case ColorReceiverType.Module:
                        color = context.ColorDic[colorType].module;
                        break;
                    case ColorReceiverType.PanelShadow:
                        color = context.ColorDic[colorType].panelShadow;
                        break;
                }

                spriteRenderer.sharedMaterial.SetColor("_Color", color);

                if (receiverType == ColorReceiverType.Panel)
                    spriteRenderer.sharedMaterial.SetVector("_ReflectionOffset",
                        layer % 2 == 0 ? new Vector4(0.01f, 0.01f, 0, 0) : Vector4.zero);
            }
        }

        public Color GetColor()
        {
            return context.ColorDic[colorType].panel;
        }

        public void BreakPanel()
        {
            if (receiverType == ColorReceiverType.Panel)
                spriteRenderer.sharedMaterial.SetFloat("_BreakScale", 1);
        }

        private void OnDestroy()
        {
            if (spriteRenderer != null)
            {
                var mat = spriteRenderer.sharedMaterial;
                if (mat != null)
                    Destroy(mat);
            }
        }
    }
}