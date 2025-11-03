using Decoration;
using Farm.View;
using Gameplay;
using UnityEngine;

namespace Farm.Model
{
    public partial class FarmModel
    {
        private bool _isAddListener = false;
        
        public void TriggerFarmGuide()
        {
            if (!_isAddListener)
            {
                EventDispatcher.Instance.AddEventListener(EventEnum.STORY_MOVIE_FINISH, StoryMovieFinish);
                _isAddListener = true;
            }
            
           if (!IsFarmModel())
           {
               CheckFarmOpen();
               return;
           }
       
           // if(CheckStory())
           //     return;
           CheckSpeedUp();
           CheckDecoButton();
           CheckTouchGround();
           CheckOrder();
        }

        private bool CheckFarmOpen()
        {
            if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Home && SceneFsm.mInstance.GetCurrSceneType() != StatusType.BackHome)
                return false;
            
            if (!IsUnLock())
                return false;

            if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Farm_Enter))
                return false;

            return GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Farm_Enter, "");
        }

        // private bool CheckStory()
        // {
        //     if (DecoManager.Instance.CurrentWorld == null)
        //         return false;
        //
        //     if (StorySubSystem.Instance.IsStoryFinished(20000))
        //         return false;
        //     
        //     StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.Initiative, "10008")
        //     var isTrigger =  StorySubSystem.Instance.Trigger(StoryTrigger.EnterMap, DecoManager.Instance.CurrentWorld.Id.ToString(), b =>
        //     {
        //         if (b)
        //         {
        //             TriggerFarmGuide();
        //         }
        //     });
        //
        //     return isTrigger;
        // }

        private bool CheckDecoButton()
        {
            if (DecoManager.Instance.CurrentWorld == null)
                return false;

            if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TouchBubble))
                return false;
         
            var areaNodes = DecoManager.Instance.CurrentWorld.GetUnlockAndNotOwnedNodes();
            if (areaNodes == null || areaNodes.Count == 0)
                return false;
   
            foreach (var area in areaNodes)
            {
                for(int i = 0; i < area.Value.Count; i++)
                {
                    if (!area.Value[i].IsOwned && !UserData.Instance.CanAford((UserData.ResourceId)area.Value[i]._data._config.costId, area.Value[i]._data._config.price))
                        continue;
                
                    if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TouchBubble, area.Value[i].Config.id.ToString()))
                        return true;
                
                    if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TouchBubble, area.Value[i].Config.costId.ToString()))
                        return true;
                }
            }

            return false;
        }
        private int[] _guideNodeId = new[]
        {
            901001,
            201101,
            201201
        };

        public bool IsGuideNode(int id)
        {
            return _guideNodeId.Contains(id);
        }
        
        private bool CheckTouchGround()
        {
            if (DecoManager.Instance.CurrentWorld == null)
                return false;

            if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Farm_TouchGround))
                return false;

            foreach (var id in _guideNodeId)
            {
                var node = DecoManager.Instance.FindNode(id);
                if(node == null)
                    continue;
                
                if(!node.IsOwned)
                    continue;
                
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Farm_TouchGround, id.ToString(),id.ToString()))
                {
                    DecoManager.Instance.CurrentWorld.FocusNode(node, () =>
                    {
                    });

                    return true;
                }
            }
            
            return true;
        }

        public void ForceFinishSpeedGuide(bool forceNext = false)
        {
            if(!GuideSubSystem.Instance.isFinished(4436))
                GuideSubSystem.Instance.ForceFinished(4436);
            
            if(!GuideSubSystem.Instance.isFinished(4437))
                GuideSubSystem.Instance.ForceFinished(4437);
            
            if(!GuideSubSystem.Instance.isFinished(4438))
                GuideSubSystem.Instance.ForceFinished(4438);

            if (forceNext)
            {
                if(!GuideSubSystem.Instance.isFinished(4439))
                    GuideSubSystem.Instance.ForceFinished(4439);
            }
        }
        private bool CheckSpeedUp()
        {
            if (DecoManager.Instance.CurrentWorld == null)
                return false;

            if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Farm_Speed))
                return false;

            var uiWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIFarmMain);
            if (uiWindow == null)
                return false;

            var status = GetGroundProductStatus(DecoManager.Instance.FindNode(901001));
            if (status == FarmProductStatus.Finish)
            {
                ForceFinishSpeedGuide();
                return false;
            }
            
            if (!((UIFarmMainController)(uiWindow)).GetCombineMono<UIFarmMain_Control>().IsSelectNode())
                return false;
            
            if(!AnimControlManager.Instance.IsShow(AnimKey.Farm_Contrl))
                return false;

            return GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Farm_Speed, "");
        }

        private bool CheckOrder()
        {
            if (DecoManager.Instance.CurrentWorld == null)
                return false;
            var uiWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIFarmMain);
            if (uiWindow == null)
                return false;
            var targets = ((UIFarmMainController)(uiWindow)).GetCombineMono<UIFarmMain_Order>()._orderCells;
            if (targets.Count == 0)
                return false;
            var hasFinishIcon = false;
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i]._cellNeed.IsEnough())
                {
                    hasFinishIcon = true;
                    break;
                }
            }
            if (!hasFinishIcon)
                return false;
            if (GuideSubSystem.Instance.isFinished(GuideTriggerPosition.Farm_FinishTask))
                return false;
            
            return GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Farm_FinishTask, "");
        }

        private void StoryMovieFinish(BaseEvent e)
        {
            if(e == null || e.datas == null || e.datas.Length == 0)
                return;

            string id = (string)e.datas[0];
            if (id == "10005")
            {
                SceneFsm.mInstance.ChangeState(StatusType.EnterFarm);
            }
            else if (id == "10015")
            {
                TriggerFarmGuide();
            }
        }
    }
}