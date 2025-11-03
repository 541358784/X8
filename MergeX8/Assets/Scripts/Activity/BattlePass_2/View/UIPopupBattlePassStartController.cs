using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;

namespace Activity.BattlePass_2
{
    public class UIPopupBattlePassStartController : UIWindowController
    {
        private Button _buttonClose;
        private Button _buttonGo;

        private LocalizeTextMeshProUGUI _timeText;

        public override void PrivateAwake()
        {
            _buttonClose = GetItem<Button>("Root/ButtonClose");
            _buttonClose.onClick.AddListener(OnBtnClose);
            _buttonGo = GetItem<Button>("Root/Button");
            _buttonGo.onClick.AddListener(OnBtnGo);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpStart);
            // _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/Text1");
            // InvokeRepeating("UpdateTimeText", 1, 1);
            // UpdateTimeText();
            InitCountDown();
        }

        // private void UpdateTimeText()
        // {
        //     _timeText.SetText(BattlePassModel.Instance.GetActivityLeftTimeString());
        // }

        private void OnBtnGo()
        {
            AnimCloseWindow(() =>
            {
                BattlePassModel.Instance.RecordShowStart();
                UIManager.Instance.OpenUI(UINameConst.UIBattlePass2Main);
            });
        }

        private void OnBtnClose()
        {
            BattlePassModel.Instance.RecordShowStart();
            AnimCloseWindow();
        }

        private LocalizeTextMeshProUGUI _countDownText;

        public void InitCountDown()
        {
            _countDownText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("UpdateActivityTime", 0.01f, 1.0f);
        }

        void UpdateActivityTime()
        {
            if (!this)
                return;
            _countDownText.SetText(BattlePassModel.Instance.GetActivityLeftTimeString());
        }

        void OnDestroy()
        {
            CancelInvoke("UpdateActivityTime");
        }
    }
}