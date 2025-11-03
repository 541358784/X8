using UnityEngine;

namespace Framework.UI.ScrollView
{
    public class CircleListCell : ScrollCell
    {
        public Transform _scaleTrans;

        private float itemPos;
        private float centerPos;
        private float disPos;
        private float maxScaleDis;
        private float minScaleDis;
        private float moveRadio;
        private float scale;

        public override void UpdateScaleTrans(Vector3 scrollPos, RectTransform centerSelectTrans, bool isHorizontal,
            bool isVertical, float selectScale, float unselectScale)
        {
            if (isHorizontal)
            {
                itemPos = scrollPos.x + GetPosX() + GetWidth() * 0.5f;
                centerPos = centerSelectTrans.localPosition.x;
                maxScaleDis = GetWidth() * 0.5f + centerSelectTrans.sizeDelta.x * 0.5f;
                minScaleDis = thisRectTranform.sizeDelta.x * 0.25f;
                disPos = Mathf.Abs(centerPos - itemPos);
            }
            else if (isVertical)
            {
                itemPos = scrollPos.y + GetPosY() - GetHeight() * 0.5f;
                centerPos = centerSelectTrans.localPosition.y;
                maxScaleDis = GetHeight() * 0.5f + centerSelectTrans.sizeDelta.y * 0.5f;
                minScaleDis = thisRectTranform.sizeDelta.y * 0.25f;
                disPos = Mathf.Abs(centerPos - itemPos);
            }

            scale = unselectScale + (selectScale - unselectScale) *
                (1 - (disPos - minScaleDis) / (maxScaleDis - minScaleDis));

            scale = scale < unselectScale ? unselectScale : scale;
            scale = scale > selectScale ? selectScale : scale;

            SetScale(scale, scale, scale);
        }

        public override void SetScale(float x, float y, float z)
        {
            if (_scaleTrans != null)
            {
                _scaleTrans.localScale = new Vector3(x, y, z);
            }
        }

        public override void SetScaleTransform(Transform trans)
        {
            _scaleTrans = trans;
        }
    }
}