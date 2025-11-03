using System;
using System.Collections.Generic;
using UnityEngine;

public partial class UIKeepPetLevelUpController
{
    public void CheckLevelRewardInfoGuide()
    {
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetLevelRewardInfo, ""))
        {
            Action<BaseEvent> EventCallback = null;
            EventCallback = (evt) =>
            {
                var guideConfig = evt.datas[0] as TableGuide;
                if (guideConfig.triggerPosition == (int) GuideTriggerPosition.KeepPetLevelRewardInfo)
                {
                    EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish,EventCallback);
                    CheckCloseLevelRewardGuide();
                }
            };
            EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,EventCallback);
        }
    }

    public void CheckCloseLevelRewardGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetCloseLevelReward))
        {
            List<Transform> topLayer = new List<Transform>();
            var target = CloseBtn;
            topLayer.Add(target.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetCloseLevelReward, target.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetCloseLevelReward, ""))
            {
            }
        }
    }

    public void FinishCloseLevelRewardGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetCloseLevelReward);
    }

    public void CheckCollectLevelRewardGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetCollectLevelReward))
        {
            var curLevel = Storage.Exp.KeepPetGetCurLevelConfig();
            if (curLevel.Id >= KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel && 
                !Storage.LevelRewardCollectState.ContainsKey(KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel))
            {
                var levelItem = LevelItemList.Find(a => a.LevelConfig.Id == KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel);
                List<Transform> topLayer = new List<Transform>();
                var target = levelItem.ResourceGroup.CollectBtn;
                topLayer.Add(target.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetCollectLevelReward, target.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetCollectLevelReward, ""))
                {
                    ScrollToTargetLevel(KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel);
                }
            }
        }
    }

    public void FinishCollectLevelRewardGuide(KeepPetLevelConfig level)
    {
        if (level.Id == KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel)
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetCollectLevelReward);
    }
}