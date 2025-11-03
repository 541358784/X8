using System;
using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    public class UIRemoveAdPopupData : UIViewParam
    {
        public int purchaseId;
    }

    [AssetAddress("TMatch/Prefabs/UINoADS")]
    public class UIRemoveAdPopup : UIPopup
    {
        public override Action EmptyCloseAction => OnCloseButtonClicked;
        [ComponentBinder("CloseButton")] public Button closeButton;
        [ComponentBinder("BuyButton")] public Button buyButton;
        [ComponentBinder("ContiuneButton")] public Button continueButton;
        [ComponentBinder("NumberText")] public TextMeshProUGUI coinNumberText;
        [ComponentBinder("BuyText")] public Text buyText;
        [ComponentBinder("Item1")] public Transform noCoinsGroup;
        [ComponentBinder("Item2")] public Transform coinsGroup;
        [ComponentBinder("DescribeText")] public LocalizeTextMeshProUGUI descText;
        [ComponentBinder("CoinIcon")] private Image coinIcon;

        private int purchaseId = 100;
        private ShopConfig shopConfig;
        private bool hasCoins = false;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            DragonPlus.GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmpackageNoadsIn);
            UIRemoveAdPopupData viewParam = param as UIRemoveAdPopupData;
            purchaseId = viewParam.purchaseId;
            // packItemData = IAPController.Instance.model.GetRemoveAdItemData(viewParam.purchaseId);
            shopConfig = TMatchConfigManager.Instance.ShopConfigList.Find((a) => a.id == purchaseId);
            ;
            hasCoins = (shopConfig.itemCnt != null && shopConfig.itemCnt.Length > 0);
            coinsGroup.gameObject.SetActive(hasCoins);
            noCoinsGroup.gameObject.SetActive(!hasCoins);
            if (hasCoins)
            {
                coinNumberText.SetText(shopConfig.itemCnt[0].ToString());
            }

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            buyButton.onClick.AddListener(OnBuyButtonClicked);
            continueButton.onClick.AddListener(OnCloseButtonClicked);

            EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.IAPSuccess, OnIAPSuccessEvent);
            EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.RestorePurchasesSuccess,
                OnRestorePurchasesSuccessEvent);
            DelayRefreshBuyText();
        }

        public async void DelayRefreshBuyText()
        {
            // await Task.Delay(10);
            descText.SetTerm("");
            // buyText.SetText(IAPController.Instance.GetPrice(packItemData.shopCfg.Id));
            buyText.text = StoreModel.Instance.GetPrice(shopConfig.id);
            if (hasCoins)
            {
                descText.SetTerm(
                    LocalizationManager.Instance.GetLocalizedStringWithFormat("ui_tm_package_removal_ad_desc1",
                        shopConfig.itemCnt[0].ToString()));
            }
            else
            {
                descText.SetTerm("ui_tm_package_removal_ad_desc2");
            }
        }

        public override Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.IAPSuccess, OnIAPSuccessEvent);
            EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.RestorePurchasesSuccess,
                OnRestorePurchasesSuccessEvent);
            return base.OnViewClose();
        }

        private void OnCloseButtonClicked()
        {
            UIViewSystem.Instance.Close<UIRemoveAdPopup>();
            LobbyTaskSystem.Instance.FinishCurrentTask();
        }

        private void OnBuyButtonClicked()
        {
            // buyButton.interactable = false;
            // IAPController.Instance.SetIAPBiParaPlacement(BiEventHappyMatchCafe.Types.MonetizationIAPEventPlacement.PlacementRemoveAdBundle);
            TMatchModel.Instance.Purchase(purchaseId, transform);
        }

        private void OnIAPSuccessEvent(BaseEvent evt)
        {
            IAPSuccessEvent drivedEvt = evt as IAPSuccessEvent;

            if (drivedEvt.shop.id == shopConfig.id && (drivedEvt.userData as UIView) == this)
            {
                DragonPlus.GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmpackageNoads,
                    data1: (shopConfig.id == 34103)?"0":"1");
                global::CommonUtils.OpenCommonConfirmWindow(new global::NoticeUIData()
                {
                    DescString = LocalizationManager.Instance.GetLocalizedString("&key.ui_ad_remove"),
                    OKCallback = () =>
                    {
                        if (shopConfig.itemId != null)
                        {
                            int id = shopConfig.itemId[0];
                            int cnt = shopConfig.itemCnt[0];
                            FlySystem.Instance.FlyItem(id, cnt,
                                coinIcon.transform.position,
                                FlySystem.Instance.GetShopTargetTransform(id).position, () =>
                                {
                                    UIViewSystem.Instance.Close<UIRemoveAdPopup>();
                                    LobbyTaskSystem.Instance.FinishCurrentTask();
                                });
                        }
                        else
                        {
                            UIViewSystem.Instance.Close<UIRemoveAdPopup>();
                            LobbyTaskSystem.Instance.FinishCurrentTask();
                        }
                    },
                    HasCloseButton = false
                });
            }
        }

        private void OnRestorePurchasesSuccessEvent(BaseEvent evt)
        {
            if (RemoveAdModel.Instance.IsRemoveAd())
            {
                UIViewSystem.Instance.Close<UIRemoveAdPopup>();
            }
        }
    }
}