using UnityEngine;

namespace Framework.UI.ScrollView
{
    public class ScrollCell : MonoBehaviour
    {
        public int Idx;
        public GameObject childObject;

        public RectTransform thisRectTranform;

        public virtual void UpdateScaleTrans(Vector3 scrollPos, RectTransform centerSelectTrans, bool isHorizontal,
            bool isVertical, float selectScale, float unselectScale)
        {
        }

        public virtual void SetScale(float x, float y, float z)
        {
        }

        public virtual float GetWidth()
        {
            return thisRectTranform.sizeDelta.x;
        }

        public virtual float GetHeight()
        {
            return thisRectTranform.sizeDelta.y;
        }

        public virtual float GetPosX()
        {
            return transform.localPosition.x;
        }

        public virtual float GetPosY()
        {
            return transform.localPosition.y;
        }

        public virtual void UpdateContentSize(Vector2 size)
        {
            thisRectTranform.sizeDelta = size;
        }

        public virtual void SetScaleTransform(Transform trans)
        {
        }
    }
}