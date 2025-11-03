//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月20日 星期五
//describe    :   
//-----------------------------------


using System;
using System.Collections;
using DragonPlus;
using UnityEngine;

namespace TMatch
{
    /// <summary>
    /// 
    /// </summary>
    public class TM_BPTipBox : UIWindowController, ICanvasRaycastFilter
    {
        private const string PREFAB_PATH = "Prefabs/Activity/TMatch/TMBP/TM_BPTipBox";
        
        
        private Transform Bg;
        private LocalizeTextMeshProUGUI _msg;
        
        bool StartRemove;
        private string defaultButtonTrim = "&key.UI_button_preview";
        public override bool EffectUIAnimation { get; set; } = false;

        public static TM_BPTipBox Open(Transform node, string msg, Vector2 offset,
            Action callBack = null, string btnTrim = null)
        {
            var ui = UIManager.Instance.GetOpenedWindow<TM_BPTipBox>();
            ui = ui ? ui : UIManager.Instance.OpenWindow<TM_BPTipBox>(PREFAB_PATH);
            ui.Init(node, msg, offset, callBack, btnTrim);
            return ui;
        }

        public override UIWindowType WindowType { get; } = UIWindowType.Popup;

        private void Init(Transform node, string msg, Vector2 offset, Action callBack, string btnTrim = null)
        {
            ShowTipMsg(msg, node, offset);
        }

        public override void PrivateAwake()
        {
            Bg = transform.Find("Root");
            _msg = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void ShowTipMsg(string msg, Transform node, Vector2 offset)
        {
            StartRemove = false;

            _msg.SetTerm(msg);

            Bg.localScale = Vector3.zero;
            Transform parentTr = Bg.parent;
            Bg.SetParent(node);
            Bg.localPosition = new Vector3(offset.x, offset.y, 0);
            Bg.localScale = Vector3.one;
            Bg.SetParent(parentTr);
            Bg.localScale = Vector3.one;
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            var localpos = CommonUtils.ScreenToCanvasPos(Bg.GetComponent<RectTransform>(), sp);
            bool isClickImage = Bg.GetComponent<RectTransform>().rect.Contains(localpos);
            if (!Input.GetMouseButtonDown(0)) return isClickImage;
            if (isClickImage || StartRemove) return isClickImage;
            StartRemove = true;
            StartCoroutine(DeleyTimeAction());

            return isClickImage;
        }

        IEnumerator DeleyTimeAction()
        {
            yield return new WaitForEndOfFrame();
            CloseWindowWithinUIMgr();
        }
    }
}