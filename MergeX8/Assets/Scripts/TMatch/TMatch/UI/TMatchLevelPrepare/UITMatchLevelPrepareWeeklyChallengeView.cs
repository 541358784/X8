
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using Item = DragonPlus.Config.Game.ItemConfig;

using ActivityWeeklyChallengeModel = WeeklyChallengeModel;

namespace TMatch
{
    public class UITMatchLevelPrepareWeeklyChallengeView : UIView
    {
        protected override bool IsChildView => true;

        [ComponentBinder("Fill")] private Image fill;
        [ComponentBinder("Icon")] private Image icon;
        [ComponentBinder("TagImage")] private Transform tagImage;

        [ComponentBinder("UICommonItem/TagImage/NumberText")]
        private LocalizeTextMeshProUGUI buffTimeText;

        [ComponentBinder("Double")] private Transform doubleGroup;
        [ComponentBinder("DescribeBG/Text")] private LocalizeTextMeshProUGUI timeText;

        private bool isUnlock = false;
        private bool hasBuff = false;

        public override void OnViewOpen(UIViewParam param)
        {
            bool isOpen = ActivityWeeklyChallengeModel.Instance.IsOpened();
            gameObject.SetActive(isOpen);
            if(!isOpen)
                return;
            
            base.OnViewOpen(param);
            isUnlock = WeeklyChallengeController.Instance.model.IsUnlock() && !WeeklyChallengeController.Instance.model.IsClaimedAll();
            gameObject.SetActive(isUnlock);
            if (!isUnlock) return;
            Refresh();
        }

        public void Refresh()
        {
            isUnlock = WeeklyChallengeController.Instance.model.IsUnlock();
            gameObject.SetActive(isUnlock);
            if (!isUnlock) return;
            WeeklyChallenge weeklyChallenge = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
            ItemConfig collectItemCfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallenge.collectItemId);
            // icon.sprite = ResourcesManager.Instance.GetSpriteVariant(collectItemCfg.Atlas, collectItemCfg.Icon);
            icon.sprite = ItemModel.Instance.GetItemSprite(collectItemCfg.id);
            hasBuff = UnlimitItemModel.Instance.IsUnlimitedItem(ItemType.TMWeeklyChallengeBuff);
            tagImage.gameObject.SetActive(hasBuff);
            doubleGroup.gameObject.SetActive(hasBuff);
            var progress = WeeklyChallengeController.Instance.model.GetCurLevelProgress();
            fill.fillAmount = Mathf.Lerp(0.1f, 0.9f, progress);
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            if (!isUnlock) return;
            if (hasBuff)
            {
                var time = UnlimitItemModel.Instance.UnlimitedItemLeftTime(ItemType.TMWeeklyChallengeBuff);
                if (time >= 0)
                {
                    buffTimeText.SetText(CommonUtils.FormatPropItemTime(time));
                }
                else
                {
                    Refresh();
                }
            }

            timeText?.SetText(WeeklyChallengeController.Instance.model.GetCurWeekLeftTimeString());
        }
    }
}