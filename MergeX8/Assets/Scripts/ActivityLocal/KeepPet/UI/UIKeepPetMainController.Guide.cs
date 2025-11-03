using System.Collections.Generic;
using UnityEngine;

public partial class UIKeepPetMainController
{
    public void CheckQuickFinishSearchingGuide()
    {
        if(!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetSearchTask3))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(QuickFinishSearchBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetSearchTask3, QuickFinishSearchBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetSearchTask3, ""))
            {
                GuideQuickFinishSearchBtn = true;
            }
        }
    }

    public void FinishQuickFinishSearchingGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetSearchTask3);
    }

    public void CheckDailyTaskEntranceGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetDailyTask1))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(DailyTaskBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetDailyTask1, DailyTaskBtn.transform as RectTransform,
                topLayer: topLayer);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetDailyTask1,"");
        }
    }

    public void FinishDailyTaskEntranceGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetDailyTask1);
    }

    // public void CheckInfoGuide()
    // {
    //     if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetInfo1,""))
    //     {
    //     }
    // }
    public void CheckWakeUpGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetWakeUp))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(AwakeBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetWakeUp, AwakeBtn.transform as RectTransform,
                topLayer: topLayer);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetWakeUp,"");
        }
    }

    public void FinishWakeUpGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetWakeUp);
    }

    // public void CheckSteakGuide()
    // {
    //     if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetPropSearch) &&
    //         Storage.SearchPropCount > 0)
    //     {
    //         List<Transform> topLayer = new List<Transform>();
    //         topLayer.Add(SearchTaskBtn.transform);
    //         GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetPropSearch, SearchTaskBtn.transform as RectTransform,
    //             topLayer: topLayer);
    //         GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetPropSearch, "");
    //     }
    // }

    // public void FinishSteakGuide()
    // {
    //     GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetPropSearch);
    // }
    
    public void CheckFeedDrumstick1Guide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetFeed1) && CurState.Enum == KeepPetStateEnum.Hunger)
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(FeedBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetFeed1, FeedBtn.transform as RectTransform,
                topLayer: topLayer);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetFeed1, "");
        }
    }

    public void FinishFeedDrumstick1Guide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetFeed1);
    }

    public void CheckDailyTaskEntrance2Guide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetDailyTask2))
        {
            var curLevel = Storage.Exp.KeepPetGetCurLevelConfig();
            if (curLevel.Id >= KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel)
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(DailyTaskBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetDailyTask2, DailyTaskBtn.transform as RectTransform,
                    topLayer: topLayer);
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetDailyTask2,"");   
            }
        }
    }

    public void FinishDailyTaskEntrance2Guide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetDailyTask2);
    }

    public void CheckSearchFinishGetRewardGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetSearchFinish1))
        {
            List<Transform> topLayer = new List<Transform>();
            var target = SearchFinishGetRewardBtn ? SearchFinishGetRewardBtn : DogButton;
            topLayer.Add(target.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetSearchFinish1, target.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetSearchFinish1, ""))
            {
            }
        }
    }

    public void FinishSearchFinishGetRewardGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetSearchFinish1);
    }

    public void CheckFeedDrumstick2Guide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetFeed5))
        {
            List<Transform> topLayer = new List<Transform>();
            var target = FeedBtn;
            topLayer.Add(target.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetFeed5, target.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetFeed5, ""))
            {
            }
        }
    }
    public void FinishFeedDrumstick2Guide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetFeed5);
    }

    public void CheckUseFrisbeeGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetFrisbee1))
        {
            List<Transform> topLayer = new List<Transform>();
            var target = FrisbeeBtn;
            topLayer.Add(target.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetFrisbee1, target.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetFrisbee1, ""))
            {
                KeepPetModel.Instance.AddDogFrisbee(1,"Guide");
            }
        }
    }
    public void FinishUseFrisbeeGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetFrisbee1);
    }

    public void CheckClickExpBarGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetClickExpBar))
        {
            List<Transform> topLayer = new List<Transform>();
            var target = LevelRewardBtn;
            topLayer.Add(target.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetClickExpBar, target.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetClickExpBar, ""))
            {
            }
        }
    }

    public void FinishClickExpBarGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetClickExpBar);
    }
    
    public void CheckClickExpBar2Guide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetClickExpBar2))
        {
            List<Transform> topLayer = new List<Transform>();
            var target = LevelRewardBtn;
            topLayer.Add(target.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetClickExpBar2, target.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetClickExpBar2, ""))
            {
            }
        }
    }

    public void FinishClickExpBar2Guide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetClickExpBar2);
    }

    public void CheckClickSearchBtnGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetClickSearchBtn) && SearchTaskBtn.gameObject.activeInHierarchy)
        {
            var curLevel = Storage.Exp.KeepPetGetCurLevelConfig();
            if (curLevel.Id >= KeepPetModel.Instance.GlobalConfig.SearchUnLockLevel)
            {
                List<Transform> topLayer = new List<Transform>();
                var target = SearchTaskBtn;
                topLayer.Add(target.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetClickSearchBtn, target.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetClickSearchBtn, ""))
                {
                }
            }
        }
    }
    public void FinishClickSearchBtnGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetClickSearchBtn);
    }
}