using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Screw.Module;
using Screw.UI;
using Screw.UIBinder;
using Screw.UserData;
using TMatch;
using TMPro;
using UnityEngine.UI;

namespace Screw
{
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupLifePurchase")]
    public class UILifePurchasePopup : UIWindowController
    {
        [UIBinder("BoosterIcon")] private Image boosterIcon;

        [UIBinder("CountText")] private LocalizeTextMeshProUGUI _countText;
        [UIBinder("BuyButton")] private Button _buyButton;
        [UIBinder("ButtonClose")] private Button _buttonClose;
        [UIBinder("BoosterCountText")] private LocalizeTextMeshProUGUI _boosterCountText;
        
        // [UIBinder("Icon")] private Image boosterIconSmall;
        [UIBinder("TitleText2")] private LocalizeTextMeshProUGUI tipTiltleTxt;
        [UIBinder("TipText")] private LocalizeTextMeshProUGUI tipTxt;

        private ScrewGameContext _tileContext;
        //错误注释
        //private CoinWidget _coinWidget;

        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);

            RegisterEvent();
        }

        public int Price => DragonPlus.Config.Screw.GameConfigManager.Instance.TableGlobalList[0].EnergyGemPrice;
        protected override void OnOpenWindow(params object[] objs)
        {
            var addValue = EnergyData.Instance.GetMaxEnergy()-EnergyData.Instance.GetEnergy();
            _boosterCountText.SetText("x"+addValue);
            _countText.SetText(Price.GetCommaFormat());
            
            tipTiltleTxt.SetTerm("ui_screw_buylife");
            tipTxt.SetTerm("ui_screw_buylife_desc");
        }


        public void RegisterEvent()
        {
            _buyButton.onClick.AddListener(OnBuyButtonClicked);
            _buttonClose.onClick.AddListener(OnCloseButtonClicked);
        }

        public void OnCloseButtonClicked()
        {
            AnimCloseWindow();
        }
        int AddCount => EnergyData.Instance.GetMaxEnergy() - EnergyData.Instance.GetEnergy();
        public void OnBuyButtonClicked()
        {
            var coinCount = Price;

            if (UserData.UserData.Instance.CanAfford(ResType.Coin, coinCount))
            {
                UserData.UserData.Instance.ConsumeRes(ResType.Coin, coinCount, new GameBIManager.ItemChangeReasonArgs() { });
                var addCount = AddCount;
                UserData.UserData.Instance.AddRes(ResType.Energy,addCount,
                    new GameBIManager.ItemChangeReasonArgs());
                FlyModule.Instance.Fly(2,addCount,boosterIcon.transform.position);
                AnimCloseWindow();
            }
            else
            {
                //错误注释
                UIScrewShop.Open(UIScrewShop.ShopViewGroupType.Coin);
            }
        }

    }
}