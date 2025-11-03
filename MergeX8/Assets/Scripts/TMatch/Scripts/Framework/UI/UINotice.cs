using System;
using System.Collections;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
namespace TMatch
{
    public class UINoticeData
    {
        /// <summary>
        /// 标题文字
        /// </summary>
        public string TitleString = string.Empty;

        /// <summary>
        /// 提示的文字
        /// </summary>
        public string DescString = "";

        /// <summary>
        /// 确认按钮的回掉,传不传都会关闭确认框
        /// </summary>
        public Action OKCallback = null;

        /// <summary>
        /// 确认按钮的文字,如果不传为OK
        /// </summary>
        public string OKButtonText = null;

        /// <summary>
        /// 是否有取消按钮
        /// </summary>
        public bool HasCancelButton = false;

        /// <summary>
        /// 取消按钮的回掉,传不传都会关闭确认框
        /// </summary>
        public Action CancelCallback = null;

        /// <summary>
        /// 取消按钮的文字,如果不传为Calcel
        /// </summary>
        public string CancelButtonText = null;

        /// <summary>
        /// 是否有关闭按钮
        /// </summary>
        public bool HasCloseButton = true;

        /// <summary>
        /// 关闭时是否有tween动画
        /// </summary>
        public bool NoTweenClose = false;

        /// <summary>
        /// 关闭时回调
        /// </summary>
        public Action CloseCallback = null;
        
        /// <summary>
        /// 是否屏蔽安卓返回键
        /// </summary>
        public bool IsLockSystemBack = false;

        /// <summary>
        /// 是否使用极高的层级,这个会比新手引导更高
        /// </summary>
        public bool IsHighSortingOrder = false;

        /// <summary>
        /// 宽度
        /// </summary>
        public float Width = 660.0f;

        /// <summary>
        /// 高度
        /// </summary>
        public float Height = 600.0f;

        /// <summary>
        /// 反转按钮位置
        /// </summary>
        public bool ReverseButton;
    }

    public class FrameWorkUINotice : UIWindow
    {
        public override UIWindowType WindowType => UIWindowType.Popup;
    
        public override UIWindowLayer WindowLayer => UIWindowLayer.Notice;
    
        public override bool EffectUIAnimation => false;
    
        public override bool UseFixedSort => true;
        public override string FixedSortName => "Top";
        public override int FixedSortOrder => 9999;
        public override bool IsUsedInTaskChokedEvent { get; } = false;
    
        public static void Open(UINoticeData data)
        {
            global::CommonUtils.OpenCommonConfirmWindow(new global::NoticeUIData
            {
                DescString = data.DescString,
                OKCallback = data.OKCallback,
                OKButtonText = data.OKButtonText,
                CancelButtonText = data.CancelButtonText,
                HasCancelButton = data.HasCancelButton,
                CancelCallback = data.CancelCallback,
            });
        }

        private RectTransform RTRoot { get; set; }
        private Button CloseButton { get; set; }
    
        LocalizeTextMeshProUGUI TitleText { get; set; }
        LocalizeTextMeshProUGUI DescText { get; set; }
        Button CancelButton { get; set; }
        LocalizeTextMeshProUGUI CancelButtonText { get; set; }
    
        Button OKButton { get; set; }
        LocalizeTextMeshProUGUI OKButtonText { get; set; }
    
        private HorizontalLayoutGroup HorizontalLayoutGroup { get; set; }
    
        UINoticeData Data { get; set; }
    
        private Animator _animator;
        private bool isTouch = false;
    
        public override void PrivateAwake()
        {
            _animator = transform.GetComponent<Animator>();
            RTRoot = transform.Find("Root").GetComponent<RectTransform>();
            CloseButton = transform.Find("Root/BGGroup/Frame/BtClose").GetComponent<Button>();
            CloseButton.onClick.AddListener(OnClickCloseButton);
    
            CancelButton = transform.Find("Root/ButtonGroup/NoButton").GetComponent<Button>();
            CancelButton.onClick.AddListener(OnClickCancelButton);
            CancelButtonText = transform.Find("Root/ButtonGroup/NoButton/Text").GetComponent<LocalizeTextMeshProUGUI>();
    
            OKButton = transform.Find("Root/ButtonGroup/YesButton").GetComponent<Button>();
            OKButton.onClick.AddListener(OnClickOKButton);
            OKButtonText = transform.Find("Root/ButtonGroup/YesButton/Text").GetComponent<LocalizeTextMeshProUGUI>();
    
            TitleText = transform.Find("Root/BGGroup/Frame/Text").GetComponent<LocalizeTextMeshProUGUI>();
            DescText = transform.Find("Root/Scroll View/Viewport/DescribeText").GetComponent<LocalizeTextMeshProUGUI>();
            
            HorizontalLayoutGroup = transform.Find("Root/ButtonGroup").GetComponent<HorizontalLayoutGroup>();
    
            OKButtonText.SetTerm("");
            CancelButtonText.SetTerm("");
        }
    
        void OnClickCancelButton()
        {
            if (isTouch) return;
            isTouch = true;
            Data.CancelCallback?.Invoke();
            DoClose();
            PlayBtnTapSound();
        }
    
        void OnClickOKButton()
        {
            if (isTouch) return;
            isTouch = true;
            Data.OKCallback?.Invoke();
            DoClose();
            PlayBtnTapSound();
        }
    
        public void SetData(UINoticeData data)
        {
            Data = data;
            ReloadUi();
        }
    
        void ReloadUi()
        {
            if (Data.IsHighSortingOrder)
                transform.GetComponent<Canvas>().sortingOrder = 9999;
            // IsLockSystemBack = Data.IsLockSystemBack;
    
            if (!string.IsNullOrEmpty(Data.TitleString))
            {
                TitleText.SetTerm("");
                TitleText.SetText(Data.TitleString);
            }
            else
            {
                TitleText.SetTerm("UI_common_notice");
            }
    
            DescText.SetTerm("");
            DescText.SetText(Data.DescString);
            CloseButton.gameObject.SetActive(Data.HasCloseButton);
            CancelButton.gameObject.SetActive(Data.HasCancelButton);
    
            if (Data.OKButtonText != null)
            {
                OKButtonText.SetText(Data.OKButtonText);
            }
            else
            {
                OKButtonText.SetTerm("&key.UI_button_ok");
            }
    
            if (Data.CancelButtonText != null)
            {
                CancelButtonText.SetText(Data.CancelButtonText);
            }
            else
            {
                CancelButtonText.SetTerm("&key.UI_button_no");
            }
    
            if (Data.Width < 500) Data.Width = 500;
            if (Data.Width > 1280) Data.Width = 1280;
            if (Data.Height < 400) Data.Height = 400;
            if (Data.Height > 700) Data.Height = 700;
            RTRoot.sizeDelta = new Vector2(Data.Width, Data.Height);
    
            HorizontalLayoutGroup.reverseArrangement = Data.ReverseButton;
        }
    
        protected override void OnOpenWindow(UIWindowData data)
        {
            // CommonUtils.TweenOpen(transform.Find("Root"));
            isTouch = false;
        }
    
        private void OnClickCloseButton()
        {
            if (isTouch) return;
            isTouch = true;
            DoClose();
            PlayBtnTapSound();
        }
    
        private void DoClose()
        {
            if (Data.NoTweenClose)
                CloseWindowWithinUIMgr(true);
            else
            {
                _animator.Play("disappear");
                var length = _animator.GetCurrentAnimatorStateInfo(0).length;
                StartCoroutine(CommonUtils.DelayCall(length, () => { CloseWindowWithinUIMgr(); }));
            }
    
            // CommonUtils.TweenClose(transform.Find("Root"), () => CloseWindowWithinUIMgr(true));
            Data.CloseCallback?.Invoke();
        }
        
        protected override void OnBackButtonCallBack()
        {
            base.OnBackButtonCallBack();
            if (CloseButton.gameObject.activeSelf)
            {
                OnClickCloseButton();
                return;
            }
    
            if (CancelButton.gameObject.activeSelf)
            {
                OnClickCancelButton();
                return;
            }
            
            if (OKButton.gameObject.activeSelf)
            {
                OnClickOKButton();
                return;
            }
        }
    }
}