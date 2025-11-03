
using UnityEngine.UI;

namespace Activity.BattlePass
{
    public class UIPopupBattlePassAddDayController : UIWindowController
    {
        private Button _buttonClose;
        private Button _buttonNo;
        private Button _buttonBuy;
        Text _priceText;

        private TableShop _shop;

        public override void PrivateAwake()
        {
            _buttonClose = GetItem<Button>("Root/ButtonClose");
            _buttonClose.onClick.AddListener(OnBtnClose);
            _buttonNo = GetItem<Button>("Root/NoButton");
            _buttonNo.onClick.AddListener(OnBtnClose);
            _buttonBuy = GetItem<Button>("Root/BuyButton");
            _buttonBuy.onClick.AddListener(OnBtnBuy);
            _priceText = GetItem<Text>("Root/BuyButton/Text1");
            InitUI();
        }

        public void InitUI()
        {
            var config = BattlePassModel.Instance.BattlePassActiveConfig;
            _shop = GlobalConfigManager.Instance.GetTableShopByID(config.shopItemId);
            _priceText.text = StoreModel.Instance.GetPrice(_shop.id);
        }

        private void OnBtnClose()
        {
            AnimCloseWindow();
        }

        private void OnBtnBuy()
        {
            StoreModel.Instance.Purchase(_shop.id, "battlePass");
        }


    }
}
