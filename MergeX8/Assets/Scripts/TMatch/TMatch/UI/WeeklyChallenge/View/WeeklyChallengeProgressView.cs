using DragonPlus;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class WeeklyChallengeProgressView : UIView
    {
        protected override bool IsChildView => true;

        [ComponentBinder("")] public Slider slider;
        [ComponentBinder("Background")] private Image background;
        [ComponentBinder("Fill")] private Image fill;
        [ComponentBinder("PlanText")] public LocalizeTextMeshProUGUI planText;
        [ComponentBinder("TargetIcon")] public Transform targetIconGroup;
        [ComponentBinder("TargetIcon/Icon")] public Image targetIcon;

        [ComponentBinder("TargetIcon/TagImage/NumberText")]
        public LocalizeTextMeshProUGUI targetIconBuffTime;

        [ComponentBinder("RewardGroup")] private Transform rewardGroup;

        private Color grayColor = new Color(0.5f, 0.5f, 0.5f);
        public WeeklyChallengeItemView rewardView;
        public bool hasBuff = false;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            Refresh();
        }

        public void Refresh()
        {
            WeeklyChallenge weeklyChallenge = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
            WeeklyChallengeReward reward = WeeklyChallengeController.Instance.model.GetCurLevelReward();
            var collectItemCfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallenge.collectItemId);
            var rewardItemCfg = TMatchShopConfigManager.Instance.GetItem(reward.rewardId);
            // targetIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(collectItemCfg.Atlas, collectItemCfg.Icon);
            targetIcon.sprite = ItemModel.Instance.GetItemSprite(collectItemCfg.id);

            hasBuff = UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMWeeklyChallengeBuff);
            targetIconGroup.Find("TagImage").gameObject.SetActive(hasBuff);
            targetIconGroup.Find("Double").gameObject.SetActive(hasBuff);

            slider.value = WeeklyChallengeController.Instance.model.GetCurLevelProgress();
            planText.SetText(WeeklyChallengeController.Instance.model.GetCurLevelProgressString());

            WeeklyChallengeItemViewParam itemViewParam = new WeeklyChallengeItemViewParam();
            itemViewParam.data = new ItemData();
            itemViewParam.data.id = rewardItemCfg.GetItemType() == ItemType.TMWeeklyChallengeBuff ? collectItemCfg.id : rewardItemCfg.id;
            itemViewParam.data.cnt = reward.rewardCnt;
            if (rewardItemCfg.GetItemType() == ItemType.TMWeeklyChallengeBuff)
            {
                itemViewParam.buffData = new ItemData();
                itemViewParam.buffData.id = rewardItemCfg.id;
            }

            rewardView = AddChildView<WeeklyChallengeItemView>(rewardGroup.gameObject, itemViewParam);
        }

        public void SetColor(bool gray)
        {
            targetIcon.color = !gray ? Color.white : grayColor;
            background.color = !gray ? Color.white : grayColor;
            fill.color = !gray ? Color.white : grayColor;
            planText.GetComponent<TextMeshProUGUI>().color = !gray ? Color.white : grayColor;
            rewardView.SetColor(gray);
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            if (hasBuff)
            {
                var time = UnlimitItemModel.Instance.UnlimitedItemLeftTime(ItemType.TMWeeklyChallengeBuff);
                if (time > 0)
                {
                    targetIconBuffTime.SetText(CommonUtils.FormatPropItemTime(time));
                }
                else
                {
                    Refresh();
                }
            }
        }
    }
}