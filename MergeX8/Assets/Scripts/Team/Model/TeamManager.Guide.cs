using System.Collections.Generic;
using Farm.Model;
using Farm.View;
using UnityEngine;

namespace Scripts.UI
{
    public partial class TeamManager
    {
        public bool CanShowEntranceGuide()//公会入口 4580
        {
            return CanShowCommonEntranceGuide(GuideTriggerPosition.TeamEntrance,
                GuideTargetType.TeamEntrance);
        }

        public bool CanShowCommonEntranceGuide(GuideTriggerPosition position, GuideTargetType targetType)
        {
            if (TeamIsUnlock() &&
                !GuideSubSystem.Instance.IsShowingGuide() &&
                !GuideSubSystem.Instance.isFinished(position))
            {
                if ((SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome ||
                    SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home) && !FarmModel.Instance.IsFarmModel())
                {
                    var auxItem = UIHomeMainController.mainController.TeamEntrance;
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
                else if ((SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome ||
                          SceneFsm.mInstance.GetCurrSceneType() == StatusType.EnterFarm) && FarmModel.Instance.IsFarmModel())
                {
                    var auxItem = UIFarmMainController.Instance?.GetCombineMono<UIFarmMain_Control>().TeamEntrance;
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
            }

            return false;
        }
    }
}