using System.Collections.Generic;
using ActivityLocal.KeepPet.UI;
using DG.Tweening;
using Dynamic;
using UnityEngine;

public partial class KeepPetModel
{
    public bool CanShowGuide()
    {
        return CanShowCommonEntranceGuide(GuideTriggerPosition.KeepPetEntrance1, GuideTargetType.KeepPetEntrance1);
    }

    public void CheckFeedGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetFeed4) && 
            !GuideSubSystem.Instance.IsShowingGuide() &&
            CurState.Enum == KeepPetStateEnum.Hunger && 
            Storage.MedicineCount > 0)
        {
            var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(CanShowFeedGuide,new[] {UINameConst.UIGuidePortrait});
            AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup);
        }
    }
    public bool CanShowFeedGuide()
    {
        return CanShowCommonEntranceGuide(GuideTriggerPosition.KeepPetFeed4, GuideTargetType.KeepPetFeed4);
    }
    
    
    
    public void CheckFrisbeeEnoughLevelUpGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetEntrance2) && 
            !GuideSubSystem.Instance.IsShowingGuide() &&
            CurState.Enum == KeepPetStateEnum.Happy)
        {
            var curLevel = Storage.Exp.KeepPetGetCurLevelConfig();
            var nextLevelNeed = curLevel.GetNextLevelNeedExp();
            var curExp = curLevel.GetCurLevelExp(Storage.Exp);
            var distance = nextLevelNeed - curExp;
            var frisbeeToExp = Storage.FrisbeeCount * GlobalConfig.FrisbeeExpValue;
            if (frisbeeToExp >= distance)
            {
                var autoPopup = new AutoPopupManager.AutoPopupManager.AutoPopUI(CanShowFrisbeeEnoughLevelUpGuide,new[] {UINameConst.UIGuidePortrait});
                AutoPopupManager.AutoPopupManager.Instance.PushExtraPopup(autoPopup);   
            }
        }
    }
    public bool CanShowFrisbeeEnoughLevelUpGuide()
    {
        return CanShowCommonEntranceGuide(GuideTriggerPosition.KeepPetEntrance2, GuideTargetType.KeepPetEntrance2);
    }
    
    public bool CanShowHungryGuide()
    {
        if (!IsOpen())
            return false;
        
        if (CurState.Enum == KeepPetStateEnum.Hunger && 
            Storage.MedicineCount <= 0)
            return CanShowCommonEntranceGuide(GuideTriggerPosition.KeepPetEntrance3, GuideTargetType.KeepPetEntrance3);
        return false;
    }
    
    public bool CanShowCommonEntranceGuide(GuideTriggerPosition position,GuideTargetType targetType)
    {
        if (IsOpen() &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(position))
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome ||
                SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home)
            {
                return false;
                var auxItem = Storage.GetAuxItem();
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(auxItem.transform);
                GuideSubSystem.Instance.RegisterTarget(targetType, auxItem.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(position, null))
                {
                    return true;
                }  
            }
            else if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_KeepPet>();
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(auxItem.transform);
                GuideSubSystem.Instance.RegisterTarget(targetType, auxItem.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(position, null))
                {
                    if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-auxItem.transform.localPosition.x+220, 0);
                    }
                    return true;
                }
            }
        }
        return false;
    }
}