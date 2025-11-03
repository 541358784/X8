using UnityEngine;

namespace StoryMovie
{
    public class ActionFactory
    {
        public static ActionBase CreateAction(TableStoryMovie config)
        {
            ActionBase actionBase = null;
            
            if (config == null)
                return null;

            switch (config.actionType)
            {
                case 0:
                {
                    actionBase = new Action_None();
                    break;
                }
                case 1:
                {
                    actionBase = new Action_PlayAnim();
                    break;
                }
                case 2:
                {
                    actionBase = new Action_Move();
                    break;
                }
                case 3:
                {
                    actionBase = new Action_Show();
                    break;
                }
                case 4:
                {
                    actionBase = new Action_PreView();
                    break;
                }
                case 5:
                {
                    actionBase = new Action_EndPreView();
                    break;
                }
                case 6:
                {
                    actionBase = new Action_AddReward();
                    break;
                }
                case 7:
                {
                    actionBase = new Action_AddTask();
                    break;
                }
                case 8:
                {
                    actionBase = new Action_ShowBubble();
                    break;
                }
                case 9:
                {
                    actionBase = new Action_CameraScale();
                    break;
                }
                case 10:
                {
                    actionBase = new Action_SetActive();
                    break;
                }
                case 11:
                {
                    actionBase = new Action_PlayVideo();
                    break;
                }
                    
                default:
                    break;
            }
            
            if(actionBase != null)
                actionBase.OnInit(config);

            return actionBase;
        }
    }
}