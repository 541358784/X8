using System.Collections.Generic;
using UnityEngine;

public partial class DogPlayModel
{
    public bool CanShowStartGuide()
    {
        GuideTriggerPosition position = GuideTriggerPosition.DogPlayUnlock;
        GuideTargetType targetType = GuideTargetType.DogPlayUnlock;
        if (LastOpenState &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(position))
        {
            UIPopupDogPlayController.Open();
            GuideSubSystem.Instance.Trigger(position, null);
            UIPopupDogPlayController.Instance.CloseEffect();
            return true;
        }
        return false;
    }
    public bool CanShowCollectGuide()
    {
        GuideTriggerPosition position = GuideTriggerPosition.DogPlayCollect;
        GuideTargetType targetType = GuideTargetType.DogPlayCollect;
        if (LastOpenState &&
            !GuideSubSystem.Instance.IsShowingGuide() &&
            !GuideSubSystem.Instance.isFinished(position))
        {
            var auxItem = MergeDogPlay.Instance;
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(auxItem.transform);
            GuideSubSystem.Instance.RegisterTarget(targetType, auxItem.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(position, null))
            {
                return true;
            }  
        }
        return false;
    }
}