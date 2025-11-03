using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
// using DragonPlus.ConfigHub.IAP;
using DragonU3DSDK.Network.API;


namespace TMatch
{
    public partial class UILobbyMainViewAdsButton : UIView
    {
        [ComponentBinder("ButtonDiscount")] private Button adsCoinButton;
        [ComponentBinder("ButtonADS")] private Button adsButton;

        protected override bool IsChildView => true;
        private static List<UILobbyMainViewAdsButton> _views = new List<UILobbyMainViewAdsButton>();
        public static void UpdateAllWindowShowState()
        {
            foreach (var view in _views)
            {
                view.UpdateShowState();
            }
        }
        public override void PrivateAwake()
        {
            base.PrivateAwake();
            _views.Add(this);
        }

        private void OnDestroy()
        {
            _views.Remove(this);
        }

        public void UpdateShowState()
        {
            gameObject.SetActive(RemoveAdModel.Instance.CanShow());
        }
        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            UpdateShowState();
            if (adsButton)
            {
                adsButton.onClick.AddListener(() => {OnAdsButtonClicked(RemoveAdModel.Instance.GetRemoveAdPackShopId(0)); });
            }

            if (adsCoinButton)
            {
                adsCoinButton.onClick.AddListener(() => {OnAdsButtonClicked(RemoveAdModel.Instance.GetRemoveAdPackShopId(1)); });

            }

            EventDispatcher.Instance.AddEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
            EventDispatcher.Instance.AddEventListener(EventEnum.RestorePurchasesSuccess, OnRestorePurchasesSuccessEvent);
        }

        public override Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RestorePurchasesSuccess, OnRestorePurchasesSuccessEvent);
            return base.OnViewClose();
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
        }

        private void OnAdsButtonClicked(int purchaseID)
        {
            UIViewSystem.Instance.Open<UIRemoveAdPopup>(new UIRemoveAdPopupData
            {
                purchaseId = purchaseID
            });
        }

        private void OnIAPSuccessEvent(BaseEvent evt)
        {
            if (RemoveAdModel.Instance.IsRemoveAd())
            {
                gameObject.SetActive(false);
            }
        }

        private void OnRestorePurchasesSuccessEvent(BaseEvent evt)
        {
            if (RemoveAdModel.Instance.IsRemoveAd())
            {
                gameObject.SetActive(false);
            }
        }
    }
}