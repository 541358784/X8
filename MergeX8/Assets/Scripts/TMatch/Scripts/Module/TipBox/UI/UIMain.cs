using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    public enum TipType
    {
        Default,
        Top,
        Left,
        Right,
    }

    public class TipBoxUIMain : UIWindowController,ICanvasRaycastFilter
    {
        public static void Open(string msg, Transform node, TipType type = TipType.Default, int offsetX = 0, int offsetY = 0,int width = 0)
        {
            string path = $"TMatch/Prefabs/";
            switch (type)
            {
                case TipType.Default:
                    path = $"{path}TipBox";
                    break;
                case TipType.Top:
                    path = $"{path}TipBoxTop2";
                    break;
                case TipType.Left:
                    path = $"{path}TipBoxLeft";
                    break;
                case TipType.Right:
                    path = $"{path}TipBoxRight";
                    break;
                default:
                    path = $"{path}TipBox";
                    break;
            }

            var tip = UIManager.Instance.OpenWindow<TipBoxUIMain>(path);
            if (tip == null) return;
            tip.ShowTipMsg(msg, node, offsetX, offsetY, width);
            tip.autoAdapt();
        }

        private RectTransform transCanvas;
        private LocalizeTextMeshProUGUI MsgText;
        Transform TextBack;
        Transform Back;
        bool StartRemove;

        RectTransform rtBackMain;
        RectTransform rtBackText;
        public System.Action closeEvent;
        public override UIWindowType WindowType { get; } = UIWindowType.Popup;

        public override UIWindowLayer WindowLayer => UIWindowLayer.Max;

        public override bool EffectUIAnimation { get; set; } = false;

        public override void PrivateAwake()
        {
            transCanvas = GetComponent<RectTransform>();
            Back = transform.Find("Input");
            TextBack = transform.Find("Input/Image");
            MsgText = transform.Find("Input/Image/Text").GetComponent<LocalizeTextMeshProUGUI>();

            rtBackMain = transform.Find("Input").GetComponent<RectTransform>();
            rtBackText = transform.Find("Input/Image").GetComponent<RectTransform>();
        }

        public void ShowTipMsg(string msg, Transform node, int offsetX = 0, int offsetY = 0, int width = 0)
        {
            StartRemove = false;

            MsgText.SetTerm(msg);
            if (width > 0)
                MsgText.GetComponent<LayoutElement>().preferredWidth = width;

            Back.localScale = Vector3.zero;
            Transform parentTr = Back.parent;
            Back.SetParent(node);
            Back.localPosition = new Vector3(offsetX, offsetY, 0);
            Back.localScale = Vector3.one;
            Back.SetParent(parentTr);
            Back.localScale = Vector3.one;
        }
        public float GetCanvasScale()
        {
            CanvasScaler mScaler = UIRoot.Instance.mRootCanvas.GetComponent<CanvasScaler>();
            return Math.Abs(mScaler.matchWidthOrHeight - 1) < 0.001f ? mScaler.referenceResolution.y / Screen.height : mScaler.referenceResolution.x / Screen.width;
        }

        public void autoAdapt()
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rtBackText);
            var screenScale = GetCanvasScale();
            var pos = RectTransformUtility.WorldToScreenPoint(UIRoot.Instance.mUICamera, rtBackText.transform.position) * screenScale;
            var rect = rtBackText.rect;

            var screenW = Screen.width * screenScale;
            var safeAreaW = Screen.safeArea.width * screenScale;
            var deltaW = screenW - safeAreaW;
            var halfW = rect.size.x / 2;
            var left = pos.x + rect.center.x - halfW;
            var right = pos.x + rect.center.x + halfW;
            float _offsetX = 0;
            if (left < deltaW / 2)
                _offsetX = deltaW / 2 - left;
            // if (right > safeAreaW)
            //     _offsetX = safeAreaW - right;
            if (right > screenW - deltaW / 2)
                _offsetX = screenW - deltaW / 2 - right;

            var screenHHalf = Screen.safeArea.height * screenScale / 2;
            // var screenHHalf = Screen.height * screenScale / 2;
            float _offsetY = 0;
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transCanvas, rtBackText);
            if (bounds.max.y > screenHHalf)
                _offsetY = screenHHalf - bounds.max.y;
            else if (bounds.min.y < -screenHHalf)
                _offsetY = -bounds.min.y - screenHHalf;

            rtBackText.anchoredPosition = new Vector2(rtBackText.anchoredPosition.x + _offsetX, rtBackText.anchoredPosition.y);
            rtBackMain.anchoredPosition = new Vector2(rtBackMain.anchoredPosition.x, rtBackMain.anchoredPosition.y + _offsetY);
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            var localpos = CommonUtils.ScreenToCanvasPos(TextBack.GetComponent<RectTransform>(), sp);
            bool isClickImage = TextBack.GetComponent<RectTransform>().rect.Contains(localpos);
            if (Input.GetMouseButtonDown(0))
                if (!isClickImage && !StartRemove)
                {
                    StartRemove = true;
                    StartCoroutine(DeleyTimeAction());
                }

            return isClickImage;
        }

        IEnumerator DeleyTimeAction()
        {
            yield return new WaitForEndOfFrame();
            CloseWindowWithinUIMgr(true);
            yield break;
        }

        private void OnDestroy()
        {
            closeEvent?.Invoke();
        }
    }
}