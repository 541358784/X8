using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;

// namespace DigTrench.UI
// {
    public class UIPopupGameTipsController:UIWindowController
    {
        public override void PrivateAwake()
        {
            var buttonNo = GetItem<Button>("Root/ButtonGroup/ButtonNo");
            var buttonOk = GetItem<Button>("Root/ButtonGroup/ButtonOk");
            var buttonClose = GetItem<Button>("Root/BgPopupBoand/ButtonClose");
            buttonNo.onClick.AddListener(OnClickCancel);
            buttonOk.onClick.AddListener(OnClickOk);
            buttonClose.onClick.AddListener(OnClickCancel);
        }

        private int _levelId;
        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            _levelId = (int)objs[0];
        }

        public void OnClickCancel()
        {
            CloseWindowWithinUIMgr(true);
        }

        public void OnClickOk()
        {
            TMatch.UILoadingEnter.Open(() =>
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameWaterLevelend,
                    data1:_levelId.ToString(),data2:"1");
                CloseWindowWithinUIMgr(true, () =>
                {
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.FinishDigTrenchGame);
                });
            });
        }
    }
// }