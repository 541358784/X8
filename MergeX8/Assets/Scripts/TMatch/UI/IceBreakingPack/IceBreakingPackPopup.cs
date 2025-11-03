using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UIIceBreakingPack")]
    public class IceBreakingPackPopup : UIPopup
    {
        // public override Action EmptyCloseAction => OnCloseButtonClicked;
        [ComponentBinder("CloseButton")] private Button closeButton;

        [ComponentBinder("Root/RewardGroup/Root")]
        private Transform reward0Group;

        [ComponentBinder("Root/RewardGroup/Root1")]
        private Transform reward1Group;

        [ComponentBinder("PayButton")] private Button payButton;

        [ComponentBinder("Root/PayButton/Text")]
        private Text priceText;

        [ComponentBinder("TitleText")] private LocalizeTextMeshProUGUI titleText;
        [ComponentBinder("DescText")] private LocalizeTextMeshProUGUI descText;

        private TMIceBreakPack packConfig;
        private TMIceBreakPackChain chainConfig;
        private List<UIItemView> itemViewList = new List<UIItemView>();
        private bool buySuccess;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            DragonPlus.GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmpackageCoinIn);
            packConfig = IceBreakingPackModel.Instance.GetShowPackConfigAndAddRecord();
            priceText.text = StoreModel.Instance.GetPrice(packConfig.ShopId);

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            payButton.onClick.AddListener(OnPayButtonClicked);
            InitRewardUI();
            ShowNameAndDesc();

            EventDispatcher.Instance.AddEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);

            // BiUtil.SendGameEvent(BiEventHappyMatchCafe.Types.GameEventType.GameEventBreakBundleShow, data1: "pop",
            //     data2: StorageManager.Instance.GetStorage<StorageGlobal>().IceBreakingPack.PopTotalTimes.ToString(),
            //     data3: packConfig.ShopId.ToString());
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            EventDispatcher.Instance.RemoveEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
            LobbyTaskSystem.Instance.FinishCurrentTask();
        }

        public override Task OnViewClose()
        {
            payButton.interactable = false;
            // BiUtil.SendGameEvent(BiEventHappyMatchCafe.Types.GameEventType.GameEventBreakBundleClose,
            //     data1: buySuccess.ToString(),
            //     data2: StorageManager.Instance.GetStorage<StorageGlobal>().IceBreakingPack.PopTotalTimes.ToString(),
            //     data3: packConfig.ShopId.ToString());
            return base.OnViewClose();
        }

        private void InitRewardUI()
        {
            reward0Group.gameObject.SetActive(packConfig.ItemIds.Count > 5);
            reward1Group.gameObject.SetActive(packConfig.ItemIds.Count <= 5);
            Transform rewardGroup = reward1Group;
            if (packConfig.ItemIds.Count > 5) rewardGroup = reward0Group;

            for (var i = 0; i < rewardGroup.childCount; i++)
            {
                var item = rewardGroup.Find("UIIceBreakingItem" + i);
                if (i < packConfig.ItemIds.Count)
                {
                    ItemViewParam data = new ItemViewParam();
                    data.data = new ItemData() {id = packConfig.ItemIds[i], cnt = packConfig.ItemNums[i]};
                    var view = AddChildView<UIItemView>(item.gameObject, data);
                    itemViewList.Add(view);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        private void ShowNameAndDesc()
        {
            var chainId = StorageManager.Instance.GetStorage<StorageTMatch>().IceBreakingPack.ChainId;
            var chain = IceBreakingPackModel.Instance.GetChainByGroup(chainId);
            if (chain != null)
            {
                titleText.SetTerm(chain.PackName);
                descText.SetTerm(chain.PackDesc);
            }
        }

        private void OnCloseButtonClicked()
        {
            UIViewSystem.Instance.Close<IceBreakingPackPopup>();
        }

        private void OnPayButtonClicked()
        {
            // IAPController.Instance.SetIAPBiParaPlacement(BiEventHappyMatchCafe.Types.MonetizationIAPEventPlacement
            //     .PlacementUnknown);
            StoreModel.Instance.Purchase(packConfig.ShopId, null);
        }

        private void OnIAPSuccessEvent(BaseEvent evt)
        {
            IAPSuccessEvent drivedEvt = evt as IAPSuccessEvent;
            if (drivedEvt.shop.id == packConfig.ShopId )
            {
                payButton.interactable = false;
                closeButton.interactable = false;
                // BiUtil.SendGameEvent(BiEventHappyMatchCafe.Types.GameEventType.GameEventBreakBundleSucess,
                //     data1: StorageManager.Instance.GetStorage<StorageGlobal>().IceBreakingPack.PopTotalTimes.ToString(),
                //     data3: packConfig.ShopId.ToString());
                Buysuccess();
            }
        }

        public void Buysuccess()
        {
            DragonPlus.GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmpackageCoin,
                data1: (packConfig.Id-1).ToString());
            for (int i = 0; i < packConfig.ItemIds.Count; i++)
            {
                int id = packConfig.ItemIds[i];
                int cnt = packConfig.ItemNums[i];
                var index = i;
                FlySystem.Instance.FlyItem(id,
                    cnt,
                    itemViewList[i].transform.position,
                    FlySystem.Instance.GetShopTargetTransform(id).position,
                    () =>
                    {
                        if (index == packConfig.ItemIds.Count - 1)
                        {
                            UIViewSystem.Instance.Close<IceBreakingPackPopup>();
                        }
                    });
            }
        }
    }
}