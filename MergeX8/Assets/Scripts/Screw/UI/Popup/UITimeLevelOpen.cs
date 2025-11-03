using DragonPlus;
using Framework;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupTimerLevel")]
    public class UITimeLevelOpen : UIWindowController
    {
        [UIBinder("ContinueButton")] private Button _continueButton;
        [UIBinder("TimeText")] private LocalizeTextMeshProUGUI _timerText;

        [UIBinder("AlarmClock")] private Transform _spineClock;

        private ScrewGameContext _tileContext;


        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);

            RegisterEvent();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _tileContext = (ScrewGameContext) objs[0];
            _timerText.SetText(CommonUtils.FormatLongToTimeStr((long) _tileContext.gameTimer.timeOutTime * 1000));
        }

        public void RegisterEvent()
        {
            _continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        protected void OnContinueButtonClicked()
        {
            _tileContext.headerView.PlayTimerAppear(_spineClock);
            AnimCloseWindow();
        }
    }
}