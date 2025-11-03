using DragonPlus;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class ShopNoAdPackItemView : UIView
    {
        [ComponentBinder("TitleText")] private LocalizeTextMeshProUGUI titleText;
        [ComponentBinder("TipsText")] private LocalizeTextMeshProUGUI tipsText;
        [ComponentBinder("Tag")] private Transform tag;
        [ComponentBinder("TagText")] private LocalizeTextMeshProUGUI tagText;
        [ComponentBinder("Item1")] private Transform item1;
        [ComponentBinder("Item2")] private Transform item2;
        [ComponentBinder("Text")] private Text text;
        [ComponentBinder("BuyButton")] private Button buyButton;
        [ComponentBinder("CoinIcon")] private Image coinIcon;

        public ShopItemViewParam drivedParam;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            gameObject.GetComponent<Canvas>().sortingOrder = 503;
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
            titleText.SetText(LocalizationManager.Instance.GetLocalizedString(drivedParam.ItemData.shopCfg.name));
            tipsText.SetText(LocalizationManager.Instance.GetLocalizedString(drivedParam.ItemData.shopCfg.description));
            // if (drivedParam.ItemData.shopCfg.Market == 0 && drivedParam.ItemData.shopCfg.ShowDiscount == 0)
            // {
            //     tag.gameObject.SetActive(false);
            // }
            // else
            // {
            //     tag.gameObject.SetActive(true);
            //     if(drivedParam.ItemData.shopCfg.ShowDiscount > 0) tagText.SetText($"-{drivedParam.ItemData.shopCfg.ShowDiscount}%");
            //     else if(drivedParam.ItemData.shopCfg.Market == 1) tagText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_iap_market_1"));
            //     else if(drivedParam.ItemData.shopCfg.Market == 2) tagText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_iap_market_2"));
            //     else if(drivedParam.ItemData.shopCfg.Market == 3) tagText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_iap_market_3"));
            // }
            tagText.SetText($"-{drivedParam.ItemData.shopCfg.showDiscount}%");
            tag.gameObject.SetActive(drivedParam.ItemData.shopCfg.showDiscount != 0);
            item1.gameObject.SetActive(false);
            item2.gameObject.SetActive(false);
            if (drivedParam.ItemData.shopCfg.GetItemCounts() == null || drivedParam.ItemData.shopCfg.GetItemCounts().Count == 0)
            {
                item1.gameObject.SetActive(true);
            }
            else
            {
                item2.gameObject.SetActive(true);
                item2.transform.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText( drivedParam.ItemData.shopCfg.GetItemCounts()[0].ToString());
            }

            text.text = StoreModel.Instance.GetPrice(drivedParam.ItemData.shopCfg.id);
        }

        private void BuyOnClick()
        {
            drivedParam.buyOnClick.Invoke(drivedParam.ItemData, this);
        }

        private void OnIAPSuccessEvent(BaseEvent evt)
        {
            IAPSuccessEvent drivedEvt = evt as IAPSuccessEvent;
            if (drivedEvt.shop.id == drivedParam.ItemData.shopCfg.id && (drivedEvt.userData as UIView) == this)
            {
                FrameWorkUINotice.Open(new UINoticeData
                {
                    DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_notice5"),
                    OKCallback = () =>
                    {
                        if (drivedEvt.shop.GetItemIds() != null)
                        {
                            int id = drivedEvt.shop.GetItemIds()[0];
                            int cnt = drivedEvt.shop.GetItemCounts()[0];
                            FlySystem.Instance.FlyItem(id, cnt, coinIcon.transform.position, FlySystem.Instance.GetShopTargetTransform(id).position, null);
                        }
                    },
                    HasCloseButton = false
                });
            }
        }
    }
}