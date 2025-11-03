using System;
using System.Collections.Generic;
using System.Drawing;
using DragonPlus;
using UnityEngine;
using DragonPlus.Config.Screw;
using DragonU3DSDK.Asset;
using Screw.Module;
using Screw.UserData;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Screw
{
    public partial class UIScrewShop
    {
        public class DailyPackageItem : MonoBehaviour
        {
            private TableDailyPackageConfig Config;
            private Button BuyBtn;
            private LocalizeTextMeshProUGUI PriceText;
            private Image CoinIcon;
            private LocalizeTextMeshProUGUI CoinNumText;
            private LocalizeTextMeshProUGUI LifeTimeText;
            private Transform DefaultRow;
            private List<Transform> Rows = new List<Transform>();
            private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
            private LocalizeTextMeshProUGUI TitleText;

            public bool AwakeFlag;

            public void Awake()
            {
                if (AwakeFlag)
                    return;
                AwakeFlag = true;
                BuyBtn = transform.Find("ButtonStart").GetComponent<Button>();
                BuyBtn.onClick.AddListener(() => { global::StoreModel.Instance.Purchase(Config.Id); });
                PriceText = transform.Find("ButtonStart/Text").GetComponent<LocalizeTextMeshProUGUI>();
                CoinIcon = transform.Find("NailIconGroup/Icon").GetComponent<Image>();
                CoinNumText = transform.Find("NailIconGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
                LifeTimeText = transform.Find("liveIconGroup/TagBG/Text").GetComponent<LocalizeTextMeshProUGUI>();
                DefaultRow = transform.Find("IconGroup/1-2");
                DefaultRow.gameObject.SetActive(false);
                TitleText = transform.Find("TiteText").GetComponent<LocalizeTextMeshProUGUI>();
                EventDispatcher.Instance.AddEvent<EventScrewBuyDailyPackage>(OnBuyDailyPackage);
                EventDispatcher.Instance.AddEvent<EventScrewRefreshDailyPackage>(OnRefreshDailyPackage);
            }

            public void Init(TableDailyPackageConfig config)
            {
                Config = config;
                Awake();
                UpdateView();
            }

            private void OnDestroy()
            {
                EventDispatcher.Instance.RemoveEvent<EventScrewBuyDailyPackage>(OnBuyDailyPackage);
                EventDispatcher.Instance.RemoveEvent<EventScrewRefreshDailyPackage>(OnRefreshDailyPackage);
            }

            public void OnBuyDailyPackage(EventScrewBuyDailyPackage evt)
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
                                FlyModule.Instance.Fly(reward.id, reward.count, CoinIcon.transform.position);
                            }
                            else if (resType == ResType.EnergyInfinity)
                            {
                                FlyModule.Instance.Fly(reward.id, reward.count, LifeTimeText.transform.position);
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

            public void OnRefreshDailyPackage(EventScrewRefreshDailyPackage evt)
            {
                if (!this)
                {
                    OnDestroy();
                    return;
                }

                UpdateView();
            }

            public void UpdateView()
            {
                var oldVisibleState = gameObject.activeSelf;
                gameObject.SetActive(DailyPackageModel.Instance.CanBuy(Config));
                var newVisibleState = gameObject.activeSelf;
                if (oldVisibleState != newVisibleState)
                {
                    EventDispatcher.Instance.SendEvent(
                        new EventScrewUpdateShopViewGroup(ShopViewGroupType.DailyPackage));
                }
                TitleText.SetTerm(Config.TitleText);
                PriceText.SetText(global::StoreModel.Instance.GetPrice(Config.Id));
                if (!gameObject.activeSelf)
                    return;
                foreach (var rewardItem in RewardItems)
                {
                    DestroyImmediate(rewardItem.gameObject);
                }

                RewardItems.Clear();
                foreach (var row in Rows)
                {
                    DestroyImmediate(row.gameObject);
                }

                Rows.Clear();
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
                        else if (resType == ResType.EnergyInfinity)
                        {
                            var time = itemConfig.InfinityTime * reward.count * (long)XUtility.Second;
                            LifeTimeText.SetText(UserData.UserData.Instance.GetInfinityTimeString(time));
                        }
                        else
                        {
                            var rowLength = 2;
                            var rewardCount = RewardItems.Count;
                            var columnIndex = rewardCount % rowLength;
                            if (columnIndex == 0)
                            {
                                var newRow = Instantiate(DefaultRow, DefaultRow.parent);
                                newRow.gameObject.SetActive(true);
                                Rows.Add(newRow);
                            }

                            var row = Rows[rewardCount / rowLength];
                            var defaultItem = row.Find("RewardItem");
                            defaultItem.gameObject.SetActive(false);
                            var rewardItem = Instantiate(defaultItem, defaultItem.parent).gameObject
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