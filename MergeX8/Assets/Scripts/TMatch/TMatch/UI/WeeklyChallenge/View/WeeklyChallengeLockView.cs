using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatch;
using UnityEngine.UI;
// using GameConfigManager = TMatch.GameConfigManager;


namespace TMatch
{
    public class WeeklyChallengeViewParam : UIViewParam
    {
        public int expectWeekId;
        public bool disableUnlockText;
    }

    [AssetAddress("TMatch/Prefabs/UIWeekChallengeLock")]
    public class WeeklyChallengeLockView : UIPopup
    {
        public virtual Action EmptyCloseAction => CloseOnClick;

        [ComponentBinder("Root/MiddleGruop/TipsText")]
        private LocalizeTextMeshProUGUI unlockText;

        [ComponentBinder("NumberText")] private LocalizeTextMeshProUGUI numberText;
        [ComponentBinder("CloseButton")] private Button closeButton;
        [ComponentBinder("ContinueButton")] private Button continueButton;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            WeeklyChallengeViewParam deriveParam = param as WeeklyChallengeViewParam;
            List<WeeklyChallengeReward> rewards = TMatchConfigManager.Instance.GetWeeklyChallengeRewards(deriveParam.expectWeekId);
            WeeklyChallengeReward grandReward = rewards.Find(x => x.rewardHugeShow == 1);
            unlockText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_weekchallenge_2", TMatchConfigManager.Instance.GlobalList[0].WeeklyChallengeUnlcok.ToString()));
            if (deriveParam.disableUnlockText) unlockText.gameObject.SetActive(false);
            numberText.SetText(grandReward.rewardCnt.ToString());
            closeButton.onClick.AddListener(CloseOnClick);
            continueButton.onClick.AddListener(CloseOnClick);
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<WeeklyChallengeLockView>();
        }
    }
}