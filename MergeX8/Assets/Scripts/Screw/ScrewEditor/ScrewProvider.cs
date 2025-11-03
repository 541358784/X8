#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Screw
{
    public class ScrewProvider : MonoBehaviour
    {
        public ScrewShape screwType;

        private void OnValidate()
        {
            OnShapeChange(screwType);
        }

        public void OnShapeChange(ScrewShape shape)
        {
            var color = transform.GetComponent<ColorProvider>().colorType;
            var shapeSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Export/Screw/Textures/Screw/{color}_{shape}.png");

            transform.Find("ScrewBody/Phillips").GetComponent<SpriteRenderer>().sprite = shapeSprite;
        }
    }
}
#endif