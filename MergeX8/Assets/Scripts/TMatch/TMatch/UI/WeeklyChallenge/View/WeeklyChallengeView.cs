using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UIWeekChallenge")]
    public class WeeklyChallengeView : UIPopup
    {
        public override Action EmptyCloseAction => CloseOnClick;

        [ComponentBinder("CloseButton")] private Button closeButton;
        [ComponentBinder("HelpButton")] private Button helpButton;

        [ComponentBinder("Root/MiddleGruop/TagGroup/CoinGroup/NumberText")]
        private LocalizeTextMeshProUGUI grandText;

        [ComponentBinder("TimeText")] private LocalizeTextMeshProUGUI timeText;
        [ComponentBinder("TaskSlider")] private Transform taskSlider;
        [ComponentBinder("Content")] private Transform content;

        [ComponentBinder("UIWeekRewardItems1")]
        private Transform rewardItems1;

        [ComponentBinder("UIWeekRewardItems2")]
        private Transform rewardItems2;

        [ComponentBinder("Root/MiddleGruop/Icon")]
        private Image icon;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            rewardItems1.gameObject.SetActive(false);
            rewardItems2.gameObject.SetActive(false);

            List<WeeklyChallengeReward> rewards = WeeklyChallengeController.Instance.model.GetCurWeekRewards();
            WeeklyChallengeReward grandReward = rewards.Find(x => x.rewardHugeShow == 1);
            grandText.SetText(grandReward.rewardCnt.ToString());
            AddChildView<WeeklyChallengeProgressView>(taskSlider.gameObject);

            WeeklyChallenge weeklyChallenge = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
            for (int i = rewards.Count - 1; i >= 0; i--)
            {
                GameObject itemObj = GameObject.Instantiate(
                    WeeklyChallengeController.Instance.model.stoage.CurLevel == i + 1 ? rewardItems2.gameObject : rewardItems1.gameObject, content);
                itemObj.SetActive(true);
                itemObj.transform.Find("SerialNoText").GetComponent<TextMeshProUGUI>().SetText((i + 1).ToString());
                if (WeeklyChallengeController.Instance.model.stoage.CurLevel <= i + 1)
                {
                    var collectItemCfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallenge.collectItemId);
                    var rewardItemCfg = TMatchShopConfigManager.Instance.GetItem(rewards[i].rewardId);
                    ItemViewParam itemViewParam = new ItemViewParam();
                    itemViewParam.data = new ItemData();
                    itemViewParam.data.id = rewardItemCfg.GetItemType() == ItemType.TMWeeklyChallengeBuff ? collectItemCfg.id : rewardItemCfg.id;
                    itemViewParam.data.cnt = rewards[i].rewardCnt;
                    UIItemView itemView = AddChildView<UIItemView>(itemObj.transform.Find("RewardGroup").gameObject, itemViewParam);
                    {
                        var cfg = TMatchShopConfigManager.Instance.GetItem(itemViewParam.data.id);
                        if (cfg.GetItemInfinityIconType() == ItemInfinityIconType.None || cfg.GetItemInfinityIconType() == ItemInfinityIconType.NoTag)
                        {
                            if (!cfg.infinity) itemView.text.SetText($"x{itemViewParam.data.cnt}");
                        }
                    }
                    //如果是buff
                    if (rewardItemCfg.GetItemType() == ItemType.TMWeeklyChallengeBuff)
                    {
                        itemView.transform.Find("Coin").gameObject.SetActive(false);
                        itemView.transform.Find("Double").gameObject.SetActive(true);
                        itemView.tagImage.gameObject.SetActive(true);
                        itemView.text.gameObject.SetActive(false);

                        itemView.infiniteText.SetText(CommonUtils.FormatPropItemTime((long)(rewardItemCfg.infiniityTime * 1000)));
                    }
                }
                else
                {
                    itemObj.transform.Find("RewardGroup").gameObject.SetActive(false);
                    itemObj.transform.Find("Lock").gameObject.SetActive(false);
                    itemObj.transform.Find("ReceiveIcon").gameObject.SetActive(true);
                }
            }

            MoveTo(rewards.Count - WeeklyChallengeController.Instance.model.stoage.CurLevel - 1);

            {
                var collectItemCfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallenge.collectItemId);
                icon.sprite = ItemModel.Instance.GetItemSprite(collectItemCfg.id);
                // icon.sprite = ResourcesManager.Instance.GetSpriteVariant(collectItemCfg.Atlas, collectItemCfg.Icon);
            }

            closeButton.onClick.AddListener(CloseOnClick);
            helpButton.onClick.AddListener(HelpOnClick);
        }

        public override Task OnViewClose()
        {
            LobbyTaskSystem.Instance.FinishCurrentTask();
            return base.OnViewClose();
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            timeText.SetText(WeeklyChallengeController.Instance.model.GetCurWeekLeftTimeString());
        }

        private async void MoveTo(int index)
        {
            await Task.Yield();
            RectTransform rect = content.GetComponent<RectTransform>();
            float itemSize = rewardItems1.GetComponent<RectTransform>().rect.height + rect.GetComponent<VerticalLayoutGroup>().spacing;
            float totalSize = 3152.0f;
            float size = index * itemSize - itemSize * 0.5f;
            if (size > totalSize) size = totalSize;
            if (size < 0) size = 0;
            rect.localPosition = new Vector3(rect.localPosition.x, size, rect.localPosition.z);
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<WeeklyChallengeView>();
        }

        private void HelpOnClick()
        {
            UIViewSystem.Instance.Open<WeeklyChallengeHelpView>();
        }
    }
}