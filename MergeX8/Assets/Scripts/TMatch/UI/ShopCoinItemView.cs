using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine.UI;


namespace TMatch
{
    public class ShopCoinItemView : UIView
    {
        [ComponentBinder("Icon")] private Image icon;
        [ComponentBinder("TipsText")] private LocalizeTextMeshProUGUI tipsText;
        [ComponentBinder("Text")] private Text text;
        [ComponentBinder("BuyButton")] private Button buyButton;

        public ShopItemViewParam drivedParam;

        protected override bool IsChildView => true;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            drivedParam = param as ShopItemViewParam;
            Refresh(drivedParam);
            buyButton.onClick.AddListener(BuyOnClick);

            EventDispatcher.Instance.AddEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
        }

        public override void OnViewDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
            base.OnViewDestroy();
        }

        public void Refresh(ShopItemViewParam param)
        {
            drivedParam = param;
            icon.sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, drivedParam.ItemData.shopCfg.icon);
            tipsText.SetText($"{LocalizationManager.Instance.GetLocalizedString(drivedParam.ItemData.shopCfg.description)}\r\n{drivedParam.ItemData.shopCfg.GetItemCounts()[0]}");
            text.text = StoreModel.Instance.GetPrice(drivedParam.ItemData.shopCfg.id);
        }

        private void BuyOnClick()
        {
            drivedParam.buyOnClick.Invoke(drivedParam.ItemData, this);
        }

        private void OnIAPSuccessEvent(BaseEvent evt)
        {
            IAPSuccessEvent drivedEvt = evt as IAPSuccessEvent;
            if (drivedEvt.shop.id == drivedParam.ItemData.shopCfg.id && (drivedEvt.userData as UIView) == this && drivedEvt.shop.GetItemIds() != null)
            {
                int id = drivedEvt.shop.GetItemIds()[0];
                int cnt = drivedEvt.shop.GetItemCounts()[0];
                FlySystem.Instance.FlyItem(id, cnt, icon.transform.position, FlySystem.Instance.GetShopTargetTransform(id).position, null);
            }
        }
    }
}