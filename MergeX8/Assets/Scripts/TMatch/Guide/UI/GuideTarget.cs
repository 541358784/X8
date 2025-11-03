using UnityEngine;
using UnityEngine.UI;

namespace OutsideGuide
{
    public class GuideTarget : MonoBehaviour
    {
        public static GuideTarget Create(GameObject go)
        {
            if(go == null) return null;
            return go.GetOrCreateComponent<GuideTarget>();
        }

        public static void Delete(GameObject go)
        {
            if(go == null) return;
            var guide = go.GetComponent<GuideTarget>();
            if(guide == null) return;
            DestroyImmediate(guide);
        }
        private Canvas _canvas;
        private GraphicRaycaster _graphicRaycaster;

        private bool hasOrginCanvas;
        private bool hasOrginGraphicRaycaster;

        private bool overrideSorting;
        private int sortingOrder;
        private string sortingLayerName;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            hasOrginCanvas = _canvas != null;
            hasOrginGraphicRaycaster = _graphicRaycaster != null;
            if (_canvas != null)
            {
                overrideSorting = _canvas.overrideSorting;
                sortingOrder = _canvas.sortingOrder;
                sortingLayerName = _canvas.sortingLayerName;
            }
            else
            {
                _canvas = gameObject.AddComponent<Canvas>();
                _graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }

            _canvas.overrideSorting = true;
            _canvas.sortingOrder = 12;
            _canvas.sortingLayerName = "Guide";
        }

        private void OnDestroy()
        {
            if (!hasOrginGraphicRaycaster) Destroy(_graphicRaycaster);
            if (!hasOrginCanvas) Destroy(_canvas);
            else
            {
                _canvas.overrideSorting = overrideSorting;
                _canvas.sortingOrder = sortingOrder;
                _canvas.sortingLayerName = sortingLayerName;
            }
        }
    }
}