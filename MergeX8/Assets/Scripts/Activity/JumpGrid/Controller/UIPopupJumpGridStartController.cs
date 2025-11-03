using Decoration;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.JumpGrid
{
    public class UIPopupJumpGridStartController : UIWindowController
    {

        private Button _closeBtn;
        private Button _startBtn;

        private LocalizeTextMeshProUGUI _timeText;

        public override void PrivateAwake()
        {
            _closeBtn = GetItem<Button>("Root/ButtonClose");
            _startBtn = GetItem<Button>("Root/Button");
            
            _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
            _startBtn.onClick.AddListener(OnBtnPlay);
            _closeBtn.onClick.AddListener(OnBtnClose);
            InvokeRepeating("UpdateTimeText", 1, 1);
            UpdateTimeText();
            if (JumpGridModel.Instance.IsPreheating())
            {
                JumpGridModel.Instance.StorageJumpGrid.IsShowPreheat = true;
            }
        }

        private void UpdateTimeText()
        {
            if (JumpGridModel.Instance.IsPreheating())
            {
                _timeText.SetText(JumpGridModel.Instance.GetActivityPreheatLeftTimeString());
                JumpGridModel.Instance.StorageJumpGrid.IsShowPreheat = true;
            }
            else
            {
                _timeText.SetText(JumpGridModel.Instance.GetActivityLeftTimeString());
            }
        }

        protected override void OnOpenWindow(params object[] objs)
        {

        }

        private void OnBtnPlay()
        {
            AnimCloseWindow(() =>
            {
                if (!JumpGridModel.Instance.IsPreheating())
                {
                    JumpGridModel.Instance.StartActivity();
                    OpenGuide();
                }
            });
        }

        private void OnBtnClose()
        {

            AnimCloseWindow(() =>
            {
                if (!JumpGridModel.Instance.IsPreheating())
                {
                    JumpGridModel.Instance.StartActivity();
                    OpenGuide();
                }
            });
        }

        private void OpenGuide()
        {
            if (!GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JumpGridStart, null))
            {
                UIManager.Instance.OpenUI(UINameConst.UIJumpGridMain);
            }
        }

        public override void ClickUIMask()
        {
            OnBtnClose();
        }

    }
}