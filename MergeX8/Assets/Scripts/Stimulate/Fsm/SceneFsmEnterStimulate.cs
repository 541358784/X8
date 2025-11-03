using Decoration;
using Decoration.DynamicMap;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using OnePathSpace;
using Stimulate.Model;
using Stimulate.System;
using UnityEngine;
using AudioManager = DragonPlus.AudioManager;

namespace Stimulate.Fsm
{
    public class SceneFsmEnterStimulate : IFsmState
    {
        public StatusType Type
        {
            get { return StatusType.EnterStimulate; }
        }

        private TableStimulateSetting _config;
        private StimulateSystem _system;


        public void Enter(params object[] objs)
        {
            Input.multiTouchEnabled = false;

            UILoadingTransitionController.Hide(() => { });
            _config = (TableStimulateSetting)objs[0];

            _system = new StimulateSystem();
            _system.Enter();

            StimulateModel.Instance.InitLevel(_config);

            UIHomeMainController.HideUI(false, true);
            UIManager.Instance.SetCanvasGroupAlpha(false, false);
            var guideWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGuidePortrait);
            if (guideWindow)
            {
                guideWindow.canvasGroup.alpha = 1f;
            }

            DynamicMapManager.Instance.PauseLogic = true;

            AudioManager.Instance.StopAllMusic();

            PlayerManager.Instance.HidePlayer();
        }

        public void Update(float deltaTime)
        {
        }

        public void LateUpdate(float deltaTime)
        {
        }

        public void Exit()
        {
            _system.Exit();
            _system = null;

            UIHomeMainController.ShowUI(false, true);

            DynamicMapManager.Instance.PauseLogic = false;

            XUtility.WaitSeconds(0.1f, () =>
            {
                AudioManager.Instance.PlayMusic(1, true);
            });
            
            StimulateModel.Instance.Release();

            UIManager.Instance.SetCanvasGroupAlpha(true, false);
            Resources.UnloadUnusedAssets();

            UIManager.Instance.EnableGraphicRaycaster(true);
       
            if (!GuideSubSystem.Instance.isFinished(101))
            {
                StorySubSystem.Instance.Trigger(StoryTrigger.Asmr, "1", b =>
                {
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
                });
            }

            EventDispatcher.Instance.DispatchEvent(EventEnum.ONE_PATH_REFRESH_REDPOINT);
            
            GuideSubSystem.Instance.CloseCurrent();
        }

    }
}