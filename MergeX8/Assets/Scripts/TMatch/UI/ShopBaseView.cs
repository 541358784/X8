using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using UnityEngine;


namespace TMatch
{
    public class ShopItemViewParam : UIViewParam
    {
        public IAPItemData ItemData;
        public Action<IAPItemData, UIView> buyOnClick;
    }

    public class ShopBaseViewParam : UIViewParam
    {
        public bool onlyPopUp;
    }

    public class ShopBaseView : UIPopup
    {
        [ComponentBinder("Content")] private Transform content;

        private bool init;
        private GameObject noAdPackItemGameObject;
        private ShopRVItemView shopRvItemView;
        private List<ShopCoinItemView> shopCoinItemViews = new List<ShopCoinItemView>();
        private List<ShopPackItemView> shopPackItemViews = new List<ShopPackItemView>();

        private ShopBaseViewParam drivedParam;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            drivedParam = param as ShopBaseViewParam;
        }

        public override Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RestorePurchasesSuccess, OnRestorePurchasesSuccessEvent);

            return base.OnViewClose();
        }

        protected void Init(bool showBpBuy = false)
        {
            if (init) return;
            init = true;

            GameObject noAdPackItemPrefab = ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Prefabs/UIShopADItem", addToCache: true);
            GameObject coin2ItemPrefab = ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Prefabs/UIShopCoinItem2", addToCache: true);
            GameObject pack1ItemPrefab = ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Prefabs/UIShopPropItem1", addToCache: true);
            GameObject pack2ItemPrefab = ResourcesManager.Instance.LoadResource<GameObject>("TMatch/Prefabs/UIShopPropItem2", addToCache: true);

            if (showBpBuy && TMBPModel.Instance.ShowBPBuy())
            {
                GameObject obj = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/TMatch/TMBP/TM_BPBuyItem");
                GameObject bpObj = GameObject.Instantiate(obj, content);
                bpObj.transform.localScale = Vector3.one;
                bpObj.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                TM_BPOtherBuyEntrance buyInfo = bpObj.AddComponent<TM_BPOtherBuyEntrance>();
                buyInfo.Init((shoplevel) =>
                {
                    CloseWindowWithinUIMgr();
                    TM_BPMainView.Open(new TM_BPMainViewOpenData{ShowUnlockAnim = shoplevel == ShopLevel.Normal});
                });
            }
            
            var iapItems = TMatchModel.Instance.IAP.GetIAPItemDatas();
            foreach (var p in iapItems)
            {
                if (TMatchModel.Instance.IAP.IsInBundleList(p.shopCfg.id)) continue;
                // if(drivedParam.onlyPopUp && p.storeCfg.PopUp != 1) continue;
                var drivedItemData = TMatchModel.Instance.IAP.GetDrivedWithBundleList(p.shopCfg.id);
                if (drivedItemData == null) continue;
                if (p.shopCfg.lmtNum != 0 && TMatchModel.Instance.IAP.GetPurchasedTimes(p.shopCfg.id) >= p.shopCfg.lmtNum) continue;

                ShopItemViewParam shopItemViewParam = new ShopItemViewParam() { ItemData = drivedItemData, buyOnClick = BuyOnClick };
                if (drivedItemData.shopCfg.GetIAPShopType() == IAPShopType.Coin)
                {
                    //看广告得金币排在Coin类型的最前面
                    // if(shopRvItemView == null && 
                    //    AdLogicManager.Instance.IsRewardUnlock(eAdReward.Shop) && AdLogicManager.Instance.GetRewardLastCount(eAdReward.Shop) > 0)
                    // {
                    //     GameObject coin1ItemPrefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/UI/UIShop/UIShopCoinItem1", addToCache : true);
                    //     GameObject obj = GameObject.Instantiate(coin1ItemPrefab, content);
                    //     shopRvItemView = AddChildView<ShopRVItemView>(obj, shopItemViewParam);
                    // }

                    {
                        GameObject obj = GameObject.Instantiate(coin2ItemPrefab, content);
                        shopCoinItemViews.Add(AddChildView<ShopCoinItemView>(obj, shopItemViewParam));
                    }
                }
                else if (drivedItemData.shopCfg.GetIAPShopType() == IAPShopType.Pack)
                {
                    GameObject obj = GameObject.Instantiate(drivedItemData.shopCfg.GetShopBgType() == 1 ? pack2ItemPrefab : pack1ItemPrefab, content);
                    shopPackItemViews.Add(AddChildView<ShopPackItemView>(obj, shopItemViewParam));
                }
                else if (drivedItemData.shopCfg.GetIAPShopType() == IAPShopType.NoAdPack)
                {
                    if (!RemoveAdModel.Instance.IsRemoveAd())
                    {
                        GameObject obj;
                        if (noAdPackItemGameObject == null)
                        {
                            noAdPackItemGameObject = GameObject.Instantiate(noAdPackItemPrefab, content);
                            obj = noAdPackItemGameObject.transform.Find("Root1").gameObject;
                        }
                        else
                        {
                            obj = noAdPackItemGameObject.transform.Find("Root2").gameObject;
                        }
                        AddChildView<ShopNoAdPackItemView>(obj, shopItemViewParam);
                    }
                }
            }

            //待事件支持权重时，再移动到OnViewOpen里
            EventDispatcher.Instance.AddEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
            EventDispatcher.Instance.AddEventListener(EventEnum.RestorePurchasesSuccess, OnRestorePurchasesSuccessEvent);
        }

        private void BuyOnClick(IAPItemData itemData, UIView view)
        {
            // TMatchModel.Instance.Purchase(itemData.shopCfg.id, null, view);
            TMatchModel.Instance.Purchase(itemData.shopCfg.id, view.transform);
        }

        private void OnIAPSuccessEvent(BaseEvent evt)
        {
            IAPSuccessEvent drivedEvt = evt as IAPSuccessEvent;
            if (drivedEvt.shop.GetIAPShopType() == IAPShopType.NoAdPack)
            {
                noAdPackItemGameObject?.SetActive(false);
            }
            else
            {
                var drivedItemData = TMatchModel.Instance.IAP.GetDrivedWithBundleList(drivedEvt.shop.id);
                //礼包链
                if (drivedItemData != null && drivedEvt.shop.id != drivedItemData.shopCfg.id)
                {
                    ShopCoinItemView shopCoinItemView = shopCoinItemViews.Find(x => x.drivedParam.ItemData.shopCfg.id == drivedEvt.shop.id);
                    if (shopCoinItemView != null) shopCoinItemView.Refresh(new ShopItemViewParam() { ItemData = drivedItemData, buyOnClick = BuyOnClick });
                    ShopPackItemView shopPackItemView = shopPackItemViews.Find(x => x.drivedParam.ItemData.shopCfg.id == drivedEvt.shop.id);
                    if (shopPackItemView != null) shopPackItemView.Refresh(new ShopItemViewParam() { ItemData = drivedItemData, buyOnClick = BuyOnClick });
                }
                //RV
                // else if (shopRvItemView != null && shopRvItemView.drivedParam.ItemData.shopCfg.id == drivedEvt.shop.id)
                // {
                //     if (AdLogicManager.Instance.GetRewardLastCount(eAdReward.Shop) > 0)
                //     {
                //         shopRvItemView.Refresh(new ShopItemViewParam() { ItemData = drivedItemData, buyOnClick = BuyOnClick });
                //     }
                //     else
                //     {
                //         RemoveChildView(shopRvItemView);
                //         shopRvItemView = null;
                //     }
                // }
            }
        }

        private void OnRestorePurchasesSuccessEvent(BaseEvent evt)
        {
            if (RemoveAdModel.Instance.IsRemoveAd())
            {
                noAdPackItemGameObject?.SetActive(false);
            }
        }
    }
}