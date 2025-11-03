using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UIWeekChallengeHelp")]
    public class WeeklyChallengeHelpView : UIPopup
    {
        public override Action EmptyCloseAction => CloseOnClick;

        [ComponentBinder("CloseButton")] private Button closeButton;
        [ComponentBinder("ContinueButton")] private Button continueButton;
        [ComponentBinder("TaskSlider")] private Transform taskSlider;
        [ComponentBinder("TimeText")] private LocalizeTextMeshProUGUI timeText;

        [ComponentBinder("Root/TagGroup/CoinGroup/NumberText")]
        private TextMeshProUGUI grandText;

        [ComponentBinder("Root/MiddleGruop/Icon")]
        private Image icon;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            List<WeeklyChallengeReward> rewards = WeeklyChallengeController.Instance.model.GetCurWeekRewards();
            WeeklyChallengeReward grandReward = rewards.Find(x => x.rewardHugeShow == 1);
            grandText.SetText(grandReward.rewardCnt.ToString());
            AddChildView<WeeklyChallengeProgressView>(taskSlider.gameObject);

            {
                WeeklyChallenge weeklyChallenge = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
                var collectItemCfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallenge.collectItemId);
                // icon.sprite = ResourcesManager.Instance.GetSpriteVariant(collectItemCfg.Atlas, collectItemCfg.Icon);
                icon.sprite = ItemModel.Instance.GetItemSprite(collectItemCfg.id);
            }

            closeButton.onClick.AddListener(CloseOnClick);
            continueButton.onClick.AddListener(CloseOnClick);
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            timeText.SetText(WeeklyChallengeController.Instance.model.GetCurWeekLeftTimeString());
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<WeeklyChallengeHelpView>();
        }
    }
}