using System.Collections.Generic;
using DragonPlus;
using UnityEngine;

public partial class UIKapiScrewMainController
{
    private Transform DefaultRewardItem;
    private List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();
    public void InitRewardGroup()
    {
        DefaultRewardItem = transform.Find("Root/ContentGroup/HorizontalLayout/Item");
        DefaultRewardItem.gameObject.SetActive(false);
        UpdateLevelRewards();
    }

    public void UpdateLevelRewards()
    {
        
        foreach (var rewardItem in RewardItems)
        {
            DestroyImmediate(rewardItem.gameObject);
        }
        RewardItems.Clear();
        var levelConfig = KapiScrewModel.Instance.GetLevelConfig(Storage.BigLevel);
        MaxSmallLevelText.SetTermFormats(levelConfig.SmallLevels.Count.ToString());
        BigLevelText.SetText(LocalizationManager.Instance.GetLocalizedString("ui_CapybaraVS_Round") + " " + (levelConfig.Id+1));
        var rewards = CommonUtils.FormatReward(levelConfig.RewardId, levelConfig.RewardNum);
        foreach (var reward in rewards)
        {
            var rewardItem = Instantiate(DefaultRewardItem, DefaultRewardItem.parent).gameObject
                .AddComponent<CommonRewardItem>();
            rewardItem.gameObject.SetActive(true);
            rewardItem.Init(reward);
            RewardItems.Add(rewardItem);
        }
    }
}