using System.Collections.Generic;
using Activity.KapiTile.View;
using DG.Tweening;
using Dynamic;
using TMatch;
using UnityEngine;

public partial class KapiTileModel
{
    public bool CanShowGuide()
    {
        return CanShowCommonEntranceGuide(GuideTriggerPosition.KapiTileEntrance, GuideTargetType.KapiTileEntrance);
    }
    
    public bool CanShowCommonEntranceGuide(GuideTriggerPosition position,GuideTargetType targetType)
    {
        if (IsPrivateOpened() &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(position))
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome ||
                SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home)
            {
                var auxItem = DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Home_KapiTile>();
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
                var auxItem =DynamicEntryManager.Instance.GetDynamicEntry<DynamicEntry_Game_KapiTile>();
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