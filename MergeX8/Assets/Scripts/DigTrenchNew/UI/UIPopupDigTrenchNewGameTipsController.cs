using DragonPlus;
using DragonPlus.Config.Ditch;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;

// namespace DigTrench.UI
// {
    public class UIPopupDigTrenchNewGameTipsController:UIWindowController
    {
        public static UIPopupDigTrenchNewGameTipsController Instance;
        public static UIPopupDigTrenchNewGameTipsController Open(TableDitchLevel config)
        {
            if (Instance)
                Instance.CloseWindowWithinUIMgr(true);
            Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupDigTrenchNewGameTips,config) as UIPopupDigTrenchNewGameTipsController;
            return Instance;
        }
        public override void PrivateAwake()
        {
            var buttonNo = GetItem<Button>("Root/ButtonGroup/ButtonNo");
            var buttonOk = GetItem<Button>("Root/ButtonGroup/ButtonOk");
            var buttonClose = GetItem<Button>("Root/BgPopupBoand/ButtonClose");
            buttonNo.onClick.AddListener(OnClickCancel);
            buttonOk.onClick.AddListener(OnClickOk);
            buttonClose.onClick.AddListener(OnClickCancel);
        }

        private TableDitchLevel Config;
        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            Config = objs[0] as TableDitchLevel;
        }

        public void OnClickCancel()
        {
            CloseWindowWithinUIMgr(true);
        }

        public void OnClickOk()
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameWaterLevelend,
                data1:Config.Id.ToString(),data2:"1");
            CloseWindowWithinUIMgr(true);
            UIDigTrenchNewMainController.Instance?.CloseWindowWithinUIMgr(true);
        }
    }
// }