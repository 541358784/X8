using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class ShopPackItemView : UIView
    {
        [ComponentBinder("TitleText")] private LocalizeTextMeshProUGUI titleText;
        [ComponentBinder("Icon")] private Image icon;
        [ComponentBinder("Tag")] private Transform tag;
        [ComponentBinder("TagText")] private LocalizeTextMeshProUGUI tagText;
        [ComponentBinder("ItemsGroup")] private Transform itemsGroup;
        [ComponentBinder("Item1")] private Transform item1;
        [ComponentBinder("Items")] private Transform items;
        [ComponentBinder("Text")] private Text text;
        [ComponentBinder("BuyButton")] private Button buyButton;

        public ShopItemViewParam drivedParam;
        private List<GameObject> itemList = new List<GameObject>();

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
            foreach (var p in itemList) GameObject.Destroy(p);
            itemList.Clear();
            drivedParam = param;
            titleText.SetText(LocalizationManager.Instance.GetLocalizedString(drivedParam.ItemData.shopCfg.name));
            icon.sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, drivedParam.ItemData.shopCfg.icon);
            // if (drivedParam.ItemData.shopCfg.Market == 0 && drivedParam.ItemData.shopCfg.showDiscount == 0)
            // {
            //     tag.gameObject.SetActive(false);
            // }
            // else
            // {
            //     tag.gameObject.SetActive(true);
            //     if(drivedParam.ItemData.shopCfg.showDiscount > 0) tagText.SetText($"-{drivedParam.ItemData.shopCfg.showDiscount}%");
            //     else if(drivedParam.ItemData.shopCfg.Market == 1) tagText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_iap_market_1"));
            //     else if(drivedParam.ItemData.shopCfg.Market == 2) tagText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_iap_market_2"));
            //     else if(drivedParam.ItemData.shopCfg.Market == 3) tagText.SetText(LocalizationManager.Instance.GetLocalizedString("UI_iap_market_3"));
            // }
            if (drivedParam.ItemData.shopCfg.description != null)
            {
                tag.gameObject.SetActive(true);
                tagText.SetText($"{LocalizationManager.Instance.GetLocalizedString(drivedParam.ItemData.shopCfg.description)}");
            }
            else
            {
                tagText.SetText($"-{drivedParam.ItemData.shopCfg.showDiscount}%");
                tag.gameObject.SetActive(drivedParam.ItemData.shopCfg.showDiscount != 0);
            }

            item1.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText(drivedParam.ItemData.shopCfg.GetItemCounts()[0].ToString());
            items.gameObject.SetActive(false);
            for (int i = 1; i < drivedParam.ItemData.shopCfg.GetItemIds().Count; i++)
            {
                GameObject itemObj = GameObject.Instantiate(items.gameObject, itemsGroup);
                itemList.Add(itemObj);
                itemObj.SetActive(true);
                DragonPlus.Config.TMatchShop.ItemConfig cfg = TMatchShopConfigManager.Instance.GetItem(drivedParam.ItemData.shopCfg.GetItemIds()[i]);
                itemObj.transform.Find("Icon").GetComponent<Image>().sprite = ItemModel.Instance.GetItemSprite(cfg.id);
                itemObj.transform.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText(
                    cfg.infinity ? CommonUtils.FormatPropItemTime((long)(cfg.infiniityTime * 1000)) : drivedParam.ItemData.shopCfg.GetItemCounts()[i].ToString());
                itemObj.transform.Find("InfiniteTag").gameObject.SetActive(cfg.type == (int)ItemType.TMEnergyInfinity);
            }

            // text.text = IAPController.Instance.GetPrice(drivedParam.ItemData.shopCfg.id);
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
                //coin
                {
                    int id = drivedEvt.shop.GetItemIds()[0];
                    int cnt = drivedEvt.shop.GetItemCounts()[0];
                    FlySystem.Instance.FlyItem(id, cnt, item1.transform.position, FlySystem.Instance.GetShopTargetTransform(id).position, null);
                }
                //boost
                for (int i = 1; i < drivedParam.ItemData.shopCfg.GetItemCounts().Count; i++)
                {
                    int id = drivedEvt.shop.GetItemIds()[i];
                    int cnt = drivedEvt.shop.GetItemCounts()[i];
                    FlySystem.Instance.FlyItem(id, cnt, itemList[i - 1].transform.position, FlySystem.Instance.GetShopTargetTransform(id).position, null);
                }

                if (drivedEvt.shop.lmtNum != 0 &&
                    TMatchModel.Instance.IAP.GetPurchasedTimes(drivedEvt.shop.id) >= drivedEvt.shop.lmtNum)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}