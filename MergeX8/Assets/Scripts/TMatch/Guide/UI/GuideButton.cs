using UnityEngine;
using UnityEngine.EventSystems;

namespace OutsideGuide
{
    public class GuideButton : MonoBehaviour,IPointerClickHandler
    {
        public static GuideButton Create(GameObject go)
        {
            if(go == null) return null;
            GuideButton btn = go.GetComponent<GuideButton>();
            if(btn != null) return btn;
            btn = go.AddComponent<GuideButton>();
            return btn;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            DecoGuideManager.Instance.CompleteStep();
            Destroy(this);
        }
    }
}