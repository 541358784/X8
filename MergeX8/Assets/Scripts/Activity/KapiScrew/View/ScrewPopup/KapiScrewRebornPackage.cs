using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Screw;
using DragonU3DSDK.Asset;
using Screw.Module;
using Screw.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    public class KapiScrewRebornPackageExtraGroup : MonoBehaviour
        {
            private bool IsAwake = false;
            private KapiScrewGiftBagConfig Config;
            private Button BuyBtn;
            private LocalizeTextMeshProUGUI PriceText;
            private Transform DefaultItem;
            private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
            private List<global::CommonRewardItem> GlobalRewardItems = new List<global::CommonRewardItem>();
            private void Awake()
            {
                if (IsAwake)
                    return;
                IsAwake = true;
                BuyBtn = transform.Find("ButtonStart").GetComponent<Button>();
                BuyBtn.onClick.AddListener(() => { UIPopupKapiScrewShopController.Open();});
                PriceText = transform.Find("ButtonStart/Text").GetComponent<LocalizeTextMeshProUGUI>();
                DefaultItem = transform.Find("IconGroup/RewardItem");
                DefaultItem.gameObject.SetActive(false);
                EventDispatcher.Instance.AddEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);
            }
            private void OnSelect(BaseEvent obj)
            {
                int index =(int) obj.datas[0];
                int select =(int) obj.datas[1];
                UpdateView();
            }

            private void OnDestroy()
            {
                EventDispatcher.Instance.RemoveEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);
            }

            public void Init(KapiScrewGiftBagConfig config)
            {
                Config = config;
                Awake();
                UpdateView();
            }
            
            public void UpdateView()
            {
                PriceText.SetText(global::StoreModel.Instance.GetPrice(Config.ShopId));
                foreach (var rewardItem in RewardItems)
                {
                    DestroyImmediate(rewardItem.gameObject);
                }
                RewardItems.Clear();
                foreach (var rewardItem in GlobalRewardItems)
                {
                    DestroyImmediate(rewardItem.gameObject);
                }
                GlobalRewardItems.Clear();
                

                var rewards = KapiScrewModel.Instance.GetGiftBagRewards();
                foreach (var reward in rewards)
                {
                    if (ScrewGameModel.Instance.IsScrewResId(reward.id))
                    {
                        var rewardData = new ResData(ScrewGameModel.Instance.ChangeToScrewId(reward.id), reward.count);
                        if (UserData.UserData.ItemDic.TryGetValue(rewardData.id, out var itemConfig))
                        {
                            var resType = (ResType)itemConfig.ItemType;
                            var rewardItem = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                                .AddComponent<CommonRewardItem>();
                            rewardItem.gameObject.SetActive(true);
                            rewardItem.Init(rewardData);
                            RewardItems.Add(rewardItem);
                        }   
                    }
                    else
                    {
                        var rewardItem = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                            .AddComponent<global::CommonRewardItem>();
                        rewardItem.gameObject.SetActive(true);
                        rewardItem.Init(reward);
                        GlobalRewardItems.Add(rewardItem);
                    }
                }
            }
        }
}