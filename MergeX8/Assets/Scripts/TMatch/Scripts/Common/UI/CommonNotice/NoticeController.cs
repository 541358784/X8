/*
 * @file NoticeController
 * 通用确认框 - 包含OK和Cancel的较小的
 * @author lu
 */

using System;
using System.Threading.Tasks;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

/*
 * 快速调用粘贴
        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
        {
                       
        });
*/
namespace TMatch
{


    public class NoticeUIData
    {
        public string DescString = ""; // 提示的文字
        public Action OKCallback = null; // 确认按钮的回掉,传不传都会关闭确认框
        public string OKButtonText = null; // 确认按钮的文字,如果不传为OK
        public bool HasCancelButton = false; // 是否有取消按钮
        public Action CancelCallback = null; // 取消按钮的回掉,传不传都会关闭确认框
        public string CancelButtonText = null; // 取消按钮的文字,如果不传为Calcel
        public bool HasCloseButton = true; // 是否有关闭按钮

        /// <summary>
        /// 关闭回调
        /// </summary>
        public Action CloseCallback = null;

        public bool NoTweenClose = false; // 关闭时是否有tween动画
        public bool IsLockSystemBack = false; // 是否屏蔽安卓返回键
        public bool IsHighSortingOrder = false; // 是否使用极高的层级,这个会比新手引导更高
        public bool closeForDefault = true; //是否在确认或取消时关闭对话框
    }

    public class NoticeController : UIWindow
    {
        public override UIWindowLayer WindowLayer => UIWindowLayer.Notice;

        public static bool IsLockSystemBack = false;

        private Button CloseButton { get; set; }

        LocalizeTextMeshProUGUI DescText { get; set; }
        Button CancelButton { get; set; }
        LocalizeTextMeshProUGUI CancelButtonText { get; set; }

        Button OKButton { get; set; }
        LocalizeTextMeshProUGUI OKButtonText { get; set; }

        public NoticeUIData Data { get; set; }

        // 唤醒界面时调用(创建的时候加载一次)
        public override void PrivateAwake()
        {
            CloseButton = transform.Find("notice/Title/close").GetComponent<Button>();
            CloseButton.onClick.AddListener(OnClickCloseButton);

            CancelButton = transform.Find("notice/ButtonObject/MapButton").GetComponent<Button>();
            CancelButton.onClick.AddListener(OnClickCancelButton);
            CancelButtonText = transform.Find("notice/ButtonObject/MapButton/BtnImage/Text")
                .GetComponent<LocalizeTextMeshProUGUI>();

            OKButton = transform.Find("notice/ButtonObject/RetryButton").GetComponent<Button>();
            OKButton.onClick.AddListener(OnClickOKButton);
            OKButtonText = transform.Find("notice/ButtonObject/RetryButton/BtnImage/Text")
                .GetComponent<LocalizeTextMeshProUGUI>();

            DescText = transform.Find("notice/Text/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        void OnClickCancelButton()
        {
            if (Data.CancelCallback != null)
                Data.CancelCallback();

            if (Data.closeForDefault)
            {
                OnClickCloseButton();
            }
        }

        void OnClickOKButton()
        {
            if (Data.OKCallback != null)
                Data.OKCallback();
            if (Data.closeForDefault)
            {
                OnClickCloseButton();
            }

            setOkButtonStatusAsync();
        }

        private async void setOkButtonStatusAsync()
        {
            if (OKButton) OKButton.enabled = false;
            await Task.Delay(1000);
            if (OKButton) OKButton.enabled = true;
        }

        public void SetData(NoticeUIData data)
        {
            Data = data;

            ReloadUI();
        }

        void ReloadUI()
        {
            IsLockSystemBack = Data.IsLockSystemBack;

            DescText.SetText(Data.DescString);
            CloseButton.gameObject.SetActive(Data.HasCloseButton);
            CancelButton.gameObject.SetActive(Data.HasCancelButton);
            OKButton.enabled = true;

            if (Data.OKButtonText != null)
            {
                OKButtonText.SetText(Data.OKButtonText);
            }
            else
            {
                OKButtonText.SetText(LocalizationManager.Instance.GetLocalizedString("&key.UI_button_ok"));
            }

            if (Data.CancelButtonText != null)
            {
                CancelButtonText.SetText(Data.CancelButtonText);
            }
            else
            {
                CancelButtonText.SetText(LocalizationManager.Instance.GetLocalizedString("&key.UI_button_no"));
            }
        }

        // 打开界面时调用(每次打开都调用)
        protected override void OnOpenWindow(UIWindowData data)
        {

        }


        private void OnClickCloseButton()
        {
            OnClose();
        }

        /// <summary>
        /// 当关闭时调用
        /// </summary>
        private void OnClose()
        {
            CloseWindowWithinUIMgr(true);
            if (null != Data.CloseCallback)
            {
                Data.CloseCallback();
            }
        }

        private void OnDestroy()
        {
            IsLockSystemBack = false;
        }
    }
}