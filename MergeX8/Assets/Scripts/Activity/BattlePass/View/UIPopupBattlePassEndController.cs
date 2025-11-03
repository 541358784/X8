using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

namespace Activity.BattlePass
{
    public class UIPopupBattlePassEndController : UIWindowController
    {
        private Button _buttonClose;
        private Button _buttonGo;
        private StorageBattlePass _storageBattlePass;

        public override void PrivateAwake()
        {
            _buttonClose = GetItem<Button>("Root/ButtonClose");
            _buttonClose.onClick.AddListener(OnBtnGo);
            _buttonGo = GetItem<Button>("Root/Button");
            _buttonGo.onClick.AddListener(OnBtnGo);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            if (objs != null && objs.Length > 0)
                _storageBattlePass = (StorageBattlePass)objs[0];
        }

        private void OnBtnGo()
        {
            AnimCloseWindow(() =>
            {

                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpEnd, _storageBattlePass.ActivityScore.ToString());

                _storageBattlePass.IsShowEnd = true;
                if (_storageBattlePass.IsPurchase)
                {
                    if (BattlePassModel.Instance.IsCanGetAllReward(_storageBattlePass))
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePassEndBuy, _storageBattlePass, UIPopupBattlePassEndBuyController.EndBuyOpenType.Receive);
                    }
                }
                else
                {
                    if (BattlePassModel.Instance.GetActivityLevel() > 5)
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePassEndBuy, _storageBattlePass, UIPopupBattlePassEndBuyController.EndBuyOpenType.EndBuy);
                    }
                    else
                    {
                        BattlePassModel.Instance.PopCommonReward(_storageBattlePass, null);
                    }
                }

            });
        }

    }
}