using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Screw.Module;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine.UI;

namespace Screw
{
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupBoosterUnlock")]
    public class UIBoosterUnlockPopup : UIWindowController
    {
        [UIBinder("BoosterIcon")] private Image boosterIcon;
        // [UIBinder("Icon")] private Image boosterIconSmall;
        [UIBinder("TitleText2")] private LocalizeTextMeshProUGUI tipTiltleTxt;
        [UIBinder("TipText")] private LocalizeTextMeshProUGUI tipTxt;

        private BoosterType _boosterType;
        private ScrewGameContext _context;

        private bool isClose = false;
        
        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _boosterType = (BoosterType) objs[0];
            _context = (ScrewGameContext) objs[1];

            switch (_boosterType)
            {
                case BoosterType.ExtraSlot:
                    var hole = AssetModule.Instance.GetSprite("ScrewCommonAtlas", "ui_common_icon_hole");
                    boosterIcon.sprite = hole;
                    // boosterIconSmall.sprite = hole;
                    tipTiltleTxt.SetTerm("ui_screw_level_newprop");
                    tipTxt.SetTerm("ui_screw_level_prop_1");
                    break;
                case BoosterType.BreakBody:
                    var hammer = AssetModule.Instance.GetSprite("ScrewCommonAtlas", "ui_common_icon_hammer");
                    boosterIcon.sprite = hammer;
                    // boosterIconSmall.sprite = hammer;
                    tipTiltleTxt.SetTerm("ui_screw_level_newprop");
                    tipTxt.SetTerm("ui_screw_level_prop_2");
                    break;
                case BoosterType.TwoTask:
                    var box = AssetModule.Instance.GetSprite("ScrewCommonAtlas", "ui_common_icon_box");
                    boosterIcon.sprite = box;
                    // boosterIconSmall.sprite = box;
                    tipTiltleTxt.SetTerm("ui_screw_level_newprop");
                    tipTxt.SetTerm("ui_screw_level_prop_3");
                    break;
            }
        }


        [UIBinder("PlayButton")]
        private void OnPlayBtnClicked()
        {
            if (isClose)
                return;

            isClose = true;
            UIModule.Instance.CloseWindow(typeof(UIBoosterUnlockPopup), true);
      
            UserData.UserData.Instance.AddRes(BoosterHandler.BoosterTypeToItemType(_boosterType), 1, new GameBIManager.ItemChangeReasonArgs(){});
            switch (_boosterType)
            {
                case BoosterType.BreakBody:
                    _context.boostersHandler.GetHandler<BreakBodyBoosterHandler>(_boosterType).FlyBoosterItemToTarget(_boosterType, 1);
                    break;
                case BoosterType.ExtraSlot:
                    _context.boostersHandler.GetHandler<ExtraSlotBoosterHandler>(_boosterType).FlyBoosterItemToTarget(_boosterType, 1);
                    break;
                case BoosterType.TwoTask:
                    _context.boostersHandler.GetHandler<TwoTaskBoosterHandler>(_boosterType).FlyBoosterItemToTarget(_boosterType, 1);
                    break;
            }
            _context.boostersView.OnPurchaseBooster(_boosterType,1);
        }

    }
}