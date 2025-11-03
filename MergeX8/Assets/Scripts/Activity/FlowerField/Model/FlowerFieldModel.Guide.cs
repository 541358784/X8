using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class FlowerFieldModel
{
    public bool CanShowGuide()
    {
        return CanShowCommonEntranceGuide(GuideTriggerPosition.FlowerFieldStart, GuideTargetType.FlowerFieldStart);
    }

    public bool CanShowLeaderBoardGuide()
    {
        var leaderBoardStorage = FlowerFieldLeaderBoardModel.Instance.GetLeaderBoardStorage(Storage.ActivityId);
        if (leaderBoardStorage != null && leaderBoardStorage.IsInitFromServer())
        {
            return CanShowCommonEntranceGuide(GuideTriggerPosition.FlowerFieldLeaderBoardHomeEntrance, GuideTargetType.FlowerFieldLeaderBoardHomeEntrance);
        }
        return false;
    }
    
    public bool CanShowCommonEntranceGuide(GuideTriggerPosition position,GuideTargetType targetType)
    {
        if (IsStart() &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(position))
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome ||
                SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home)
            {
                var auxItem = Aux_FlowerField.Instance;
                if (!auxItem)
                    return false;
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
                var auxItem = MergeFlowerField.Instance;
                if (!auxItem)
                    return false;
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