using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using UnityEngine.UI;

public partial class UIParrotMainController
{
    public Slider Slider;
    public LocalizeTextMeshProUGUI SliderText;
    public List<CommonRewardItem> RewardItems = new List<CommonRewardItem>();

    public void InitTopGroup()
    {
        Slider = GetItem<Slider>("Root/TopGroup/Slider");
        SliderText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/Slider/Text");
        var rewardItemIndex = 1;
        var rewardItemNode = transform.Find("Root/TopGroup/RewardGroup/Item" + rewardItemIndex);
        while (rewardItemNode)
        {
            var rewardItem = rewardItemNode.gameObject.AddComponent<CommonRewardItem>();
            RewardItems.Add(rewardItem);
            rewardItemIndex++;
            rewardItemNode = transform.Find("Root/TopGroup/RewardGroup/Item" + rewardItemIndex);
        }
        var curState = ParrotModel.Instance.GetLevelState(Storage.TotalScore);
        SetTopGroupState(curState);
    }

    public void SetTopGroupState(ParrotLevelState state)
    {
        Slider.DOKill();
        Slider.value = (float)state.CurScore / state.MaxScore;
        SliderText.SetText(state.CurScore+"/"+state.MaxScore);
        if (state.CurScore >= state.MaxScore)
        {
            SliderText.SetText("Max");
        }

        for (var i = 0; i < RewardItems.Count; i++)
        {
            if (i < state.Rewards.Count)
            {
                RewardItems[i].Init(state.Rewards[i]);
                RewardItems[i].gameObject.SetActive(true);
            }
            else
            {
                RewardItems[i].gameObject.SetActive(false);
            }
        }
    }

    public void TopGroupPerformToMax(ParrotLevelState state)
    {
        SetTopGroupState(state);
        Slider.DOValue(1f, JumpTime).OnUpdate(() =>
        {
            var curScore = (int)(Slider.value * state.MaxScore);
            SliderText.SetText(curScore + "/" + state.MaxScore);
        }).SetEase(Ease.Linear);
    }
}