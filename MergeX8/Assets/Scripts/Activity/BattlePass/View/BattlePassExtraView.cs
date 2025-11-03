using Activity.BattlePass;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay.UI;
using UnityEngine.UI;

namespace Activity.BattlePass
{
    public class BattlePassExtraView : UIPopupExtraView
    {
        public override bool CanShow()
        {
            if (UIPopupBattlePassBuyNew.CanShowUIWithOutOpenWindow())
                return true;
            return false;
        }

        private bool _isInit = false;
        private Button _buyButton;

        public override void Init()
        {
            if (_isInit)
                return;
            _isInit = true;
            _buyButton = transform.Find("Button").GetComponent<Button>();
            _buyButton.onClick.AddListener(() =>
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpclickLifeUseup, data1: "1");
                UIPopupBattlePassBuyNew.CanShowUIWithOpenWindow();
            });
            transform.Find("ItemGroup/Item1/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(BattlePassModel.Instance.BattlePassActiveConfig.buyReward[1].ToString());
            InitCountDown();
        }

        private LocalizeTextMeshProUGUI _countDownText;

        public void InitCountDown()
        {
            _countDownText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
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