using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.TimeOrder;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay.UI.Store.Vip.Model;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TimeOrder.Controller
{
    public class UIPopupTimeOrderGiftController : UIWindowController
    {
        private Button CloseBtn;
        private LocalizeTextMeshProUGUI TimeText;
        private Transform DefaultRewardItem;
        private List<CommonRewardItem> RewardItemList = new List<CommonRewardItem>();
        private Button BuyBtn;
        private Text PriceText;
        private TableTimeOrderGift _tableConfig;
        
        public override void PrivateAwake()
        {
            CloseBtn = GetItem<Button>("Root/CloseButton");
            CloseBtn.onClick.AddListener(OnClickCloseBtn);
            TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
            DefaultRewardItem = GetItem<Transform>("Root/RewardGroup/Item");
            DefaultRewardItem.gameObject.SetActive(false);
            BuyBtn = GetItem<Button>("Root/BuyButton");
            BuyBtn.onClick.AddListener(OnClickBuyBtn);
            PriceText = GetItem<Text>("Root/BuyButton/Text");
            InvokeRepeating("UpdateTime", 0, 1);
        }

        private void Start()
        {
            _tableConfig = TimeOrderConfigManager.Instance.TableTimeOrderGiftList.Find(a => a.Id == TimeOrderModel.Instance.TimeOrder.GiftId);
            if (_tableConfig == null)
                _tableConfig = TimeOrderConfigManager.Instance.TableTimeOrderGiftList.Last();
            
            var rewardList = CommonUtils.FormatReward(_tableConfig.RewardId, _tableConfig.RewardCount);
            for (var i = 0; i < rewardList.Count; i++)
            {
                var rewardConfig = rewardList[i];
                var rewardItem = Instantiate(DefaultRewardItem.gameObject, DefaultRewardItem.parent)
                    .AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(rewardConfig);
                rewardItem._clickinfo.gameObject.SetActive(false);
                RewardItemList.Add(rewardItem);
            }

            foreach (var id in TimeOrderModel.Instance.TimeOrder.OrderGiftContent)
            {
                var rewardItem = Instantiate(DefaultRewardItem.gameObject, DefaultRewardItem.parent)
                    .AddComponent<CommonRewardItem>();
                rewardItem.gameObject.SetActive(true);
                rewardItem.Init(new ResData(id, 1));
                rewardItem._clickinfo.gameObject.SetActive(false);
                rewardItem._numText.gameObject.SetActive(true);
                RewardItemList.Add(rewardItem);
            }
            PriceText.text = StoreModel.Instance.GetPrice(_tableConfig.ShopId);
            
            TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(_tableConfig.ShopId);
            GetItem<LocalizeTextMeshProUGUI>("Root/Vip/Text").SetText(VipStoreModel.Instance.GetVipScoreString(tableShop.price));
        }

        public void UpdateTime()
        {
            TimeText.SetText(TimeOrderModel.Instance.GetJoinEndTimeString());
        }

        public void OnClickBuyBtn()
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMergeactPackagePurchase);
            
            StoreModel.Instance.Purchase(_tableConfig.ShopId, param1:_tableConfig.Id);
        }

        public void OnClickCloseBtn()
        {
            AnimCloseWindow();
        }
    }
}