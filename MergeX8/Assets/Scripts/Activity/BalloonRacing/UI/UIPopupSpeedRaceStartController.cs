using System.Collections;
using System.Collections.Generic;
using Activity.BalloonRacing;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BalloonRacing.UI
{
    public class UIPopupSpeedRaceStartController : UIWindowController
    {
        private LocalizeTextMeshProUGUI _textTime;
        private Button _buttonClose;
        private Button _buttonJoin;
        private bool _isClose = false;

        public override void PrivateAwake()
        {
            _textTime = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
            _buttonClose = GetItem<Button>("Root/CloseButton");
            _buttonJoin = GetItem<Button>("Root/Button");
            _buttonJoin.onClick.AddListener(delegate
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BalloonRacingPlay);
                AnimCloseWindow(beforeCloseFunc:delegate
                {
                    BalloonRacingModel.Instance.Storage.IsJoin = true;
                    BalloonRacingModel.Instance.JoinOrInitRacing(true);
                    BalloonRacingModel.Instance.TryOpenMain();
                });
            });

            _buttonClose.onClick.AddListener(delegate { AnimCloseWindow(); });

            BalloonRacingModel.Instance.RecordOpenJoinRacing();
            InvokeRepeating(nameof(UpdateTime), 0, 1);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BalloonRacingItemResource, transform as RectTransform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BalloonRacingPlay, _buttonJoin.transform, topLayer: _buttonJoin.transform);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BalloonRacingItemResource, null);

            if (objs != null && objs.Length > 0)
            {
                _buttonJoin.gameObject.SetActive(false);
                _buttonClose.gameObject.SetActive(true);
            }
            else
            {
                _buttonClose.gameObject.SetActive(false);
            }
        }

        private void UpdateTime()
        {
            _textTime.SetText(BalloonRacingModel.Instance.GetActivityLeftTimeString());
            if (BalloonRacingModel.Instance.GetActivityLeftTime() <= 0 && !_isClose)
            {
                _isClose = true;
                AnimCloseWindow();
            }
        }
    }
}
