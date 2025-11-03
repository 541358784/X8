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
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupBoosterPurchase")]
    public class UIBoosterPurchasePopup : UIWindowController
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
        private BoosterHandler _boosterHandler;
        //错误注释
        //private CoinWidget _coinWidget;

        public override void PrivateAwake()
        {
            ComponentBinderUI.BindingComponent(this, transform);

            RegisterEvent();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _boosterHandler = (BoosterHandler) objs[0];
  
            _boosterCountText.SetText("x3");
            _countText.SetText(_boosterHandler.GetPurchaseCoinCount().GetCommaFormat());

            switch (_boosterHandler.BoosterType)
            {
                case BoosterType.ExtraSlot:
                    var hole = UserData.UserData.GetResourceIcon(10);
                    boosterIcon.sprite = hole;
                    // boosterIconSmall.sprite = hole;
                    tipTiltleTxt.SetTerm("ui_screw_level_buyprop");
                    tipTxt.SetTerm("ui_screw_level_prop_1");
                    break;
                case BoosterType.BreakBody:
                    var hammer = UserData.UserData.GetResourceIcon(11);
                    boosterIcon.sprite = hammer;
                    // boosterIconSmall.sprite = hammer;
                    tipTiltleTxt.SetTerm("ui_screw_level_buyprop");
                    tipTxt.SetTerm("ui_screw_level_prop_2");
                    break;
                case BoosterType.TwoTask:
                    var box = UserData.UserData.GetResourceIcon(12);
                    boosterIcon.sprite = box;
                    // boosterIconSmall.sprite = box;
                    tipTiltleTxt.SetTerm("ui_screw_level_buyprop");
                    tipTxt.SetTerm("ui_screw_level_prop_3");
                    break;
            }
            
            if(_boosterHandler.Context.gameState == ScrewGameState.InProgress 
               && _boosterHandler.Context.gameTimer != null)
                _boosterHandler.Context.gameTimer.EnableTimer(false);
            //错误注释
            // _coinWidget  = CreateWidgetByPath<CoinWidgetSingle>(gameObject.transform,"CurrencyCoin", true);
            // _coinWidget.IgnoreCoinIcon();
        }


        public void RegisterEvent()
        {
            _buyButton.onClick.AddListener(OnBuyButtonClicked);
            _buttonClose.onClick.AddListener(OnCloseButtonClicked);
        }

        public void OnCloseButtonClicked()
        {
            if(_boosterHandler.Context.gameState == ScrewGameState.InProgress 
            && _boosterHandler.Context.gameTimer != null)
                _boosterHandler.Context.gameTimer.EnableTimer(true);
            AnimCloseWindow();
            //错误注释
            //GameApp.Get<AdSys>().TryShowCloseBanner();
        }

        public void OnBuyButtonClicked()
        {
            var coinCount = _boosterHandler.GetPurchaseCoinCount();

            if (UserData.UserData.Instance.CanAfford(ResType.Coin, coinCount))
            {
                _boosterHandler.PurchaseBooster();
                AnimCloseWindow();
                
                if(_boosterHandler.Context.gameState == ScrewGameState.InProgress 
                   && _boosterHandler.Context.gameTimer != null)
                    _boosterHandler.Context.gameTimer.EnableTimer(true);
            }
            else
            {
                UIScrewShop.Open(UIScrewShop.ShopViewGroupType.Coin);
            }
        }

    }
}