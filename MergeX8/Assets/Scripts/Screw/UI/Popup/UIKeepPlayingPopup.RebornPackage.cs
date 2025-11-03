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
    public partial class UIKeepPlayingPopup
    {
        public class RebornPackageExtraGroup : MonoBehaviour
        {
            private bool IsAwake = false;
            private TableRebornPackageConfig Config;
            private Button BuyBtn;
            private LocalizeTextMeshProUGUI PriceText;
            private Image CoinIcon;
            private LocalizeTextMeshProUGUI CoinNumText;
            private Transform DefaultItem;
            private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
            private void Awake()
            {
                if (IsAwake)
                    return;
                IsAwake = true;
                BuyBtn = transform.Find("ButtonStart").GetComponent<Button>();
                BuyBtn.onClick.AddListener(() => { global::StoreModel.Instance.Purchase(Config.Id); });
                PriceText = transform.Find("ButtonStart/Text").GetComponent<LocalizeTextMeshProUGUI>();
                CoinIcon = transform.Find("NailIconGroup/Icon").GetComponent<Image>();
                CoinNumText = transform.Find("NailIconGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
                DefaultItem = transform.Find("IconGroup/RewardItem");
                DefaultItem.gameObject.SetActive(false);
                EventDispatcher.Instance.AddEvent<EventScrewBuyRebornPackage>(OnBuyRebornPackage);
            }

            public void OnBuyRebornPackage(EventScrewBuyRebornPackage evt)
            {
                if (!this)
                {
                    OnDestroy();
                    return;
                }

                if (evt.Config == Config)
                {
                    var rewards = CommonUtils.FormatReward(Config.RewardId, Config.RewardNum);
                    var extraRewardIndex = 0;
                    foreach (var reward in rewards)
                    {
                        if (UserData.UserData.ItemDic.TryGetValue(reward.id, out var itemConfig))
                        {
                            var resType = (ResType)itemConfig.ItemType;
                            if (resType == ResType.Coin)
                            {
                                FlyModule.Instance.Fly(reward.id,reward.count, CoinIcon.transform.position);
                            }
                            else
                            {
                                var rewardItem = RewardItems[extraRewardIndex];
                                extraRewardIndex++;
                                FlyModule.Instance.Fly(reward.id, reward.count, rewardItem.transform.position);
                            }
                        }
                    }

                    UpdateView();
                }
            }
            private void OnDestroy()
            {
                EventDispatcher.Instance.RemoveEvent<EventScrewBuyRebornPackage>(OnBuyRebornPackage);
            }
            
            public void Init(TableRebornPackageConfig config)
            {
                Config = config;
                Awake();
                UpdateView();
            }
            
            public void UpdateView()
            {
                PriceText.SetText(global::StoreModel.Instance.GetPrice(Config.Id));
                foreach (var rewardItem in RewardItems)
                {
                    DestroyImmediate(rewardItem.gameObject);
                }
                RewardItems.Clear();
                var rewards = CommonUtils.FormatReward(Config.RewardId, Config.RewardNum);
                foreach (var reward in rewards)
                {
                    if (UserData.UserData.ItemDic.TryGetValue(reward.id, out var itemConfig))
                    {
                        var resType = (ResType)itemConfig.ItemType;
                        if (resType == ResType.Coin)
                        {
                            CoinIcon.sprite =
                                ResourcesManager.Instance.GetSpriteVariant("ScrewCommonAtlas", Config.CoinImage);
                            CoinNumText.SetText(reward.count.ToString());
                        }
                        else
                        {
                            var rewardItem = Instantiate(DefaultItem, DefaultItem.parent).gameObject
                                .AddComponent<CommonRewardItem>();
                            rewardItem.gameObject.SetActive(true);
                            rewardItem.Init(reward);
                            RewardItems.Add(rewardItem);
                        }
                    }
                }
            }
        }
    }
}