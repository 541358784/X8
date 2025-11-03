using DragonPlus;
using DragonU3DSDK.Network.API;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine.UI;

namespace Activity.BattlePass
{
    public class UIPopupBattlePassBuyNewUltimate : UIWindowController
    {
        private Button _buttonClose;
        private Button _ultimateBuy;
        private Text _ultimateText;
        private TableShop _ultimateShop;
        
        private LocalizeTextMeshProUGUI _ultimateScoreText;
        private LocalizeTextMeshProUGUI _ultimateMultipleText;
        private LocalizeTextMeshProUGUI _vipText;
        
        public override void PrivateAwake()
        {
            _ultimateScoreText= transform.Find("Root/ItemGroup/Reward1/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _ultimateMultipleText= transform.Find("Root/ItemGroup/Reward2/Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            _vipText = gameObject.transform.Find("Root/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            _buttonClose = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _buttonClose.onClick.AddListener(OnClose);
            
            _ultimateBuy = transform.Find("Root/Button").GetComponent<Button>();
            _ultimateBuy.onClick.AddListener(() =>
            {
                OnBtnBuy();
            });
            _ultimateText = transform.Find("Root/Button/Text").GetComponent<Text>();

            InitUI();
            EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_PURCHASE, OnCloseUI);
        }
           
        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
        }
        
        public void InitUI()
        {
            var config = BattlePassModel.Instance.BattlePassShopConfig;
            _ultimateShop = GlobalConfigManager.Instance.GetTableShopByID(config.ultimateShopId);
                
            _ultimateText.text = StoreModel.Instance.GetPrice(_ultimateShop.id);
            _ultimateScoreText.SetText("+"+config.ultimateSkipStep.ToString());
            _ultimateMultipleText.SetText("x"+((100f+config.ultimateScoreMultiple)/100f).ToString("0.0"));
            
            _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(_ultimateShop.price));
        }
        
        private void OnClose()
        {
            AnimCloseWindow();
        }
        
        private void OnBtnBuy()
        {
            StoreModel.Instance.Purchase(_ultimateShop.id, "ultimatebattlePass");
        }
        
        private void OnCloseUI(BaseEvent e)
        {
            OnClose();
        }
        
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_PURCHASE, OnCloseUI);
        }

        private static string constPlaceId = "battlepassbuypurchase";
        public static bool CanShow()
        {
            if (!BattlePassModel.Instance.CanShowUltimatePurchase())
                return false;
            
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
                return false;
        
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,
                CommonUtils.GetTimeStamp());

            UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePassBuyNew2);
            return true;
        }
    }
}