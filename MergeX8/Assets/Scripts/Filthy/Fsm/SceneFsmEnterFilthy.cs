using System;
using Decoration;
using Decoration.DynamicMap;
using DragonPlus;
using Filthy.Model;
using Filthy.System;
using UnityEngine;

namespace Filthy.Fsm
{
    public class SceneFsmEnterFilthy : IFsmState
    {
        public StatusType Type => StatusType.EnterFilthy;

        private CameraSystem _cameraSystem;

        private Action _action = null;
        
        public void Enter(params object[] objs)
        {
            if(objs != null && objs.Length > 0)
                _action = (Action)objs[0];
            
            Input.multiTouchEnabled = false;

            UILoadingTransitionController.Hide(() => { });

            _cameraSystem = new CameraSystem();
            _cameraSystem.Init();

            FilthyModel.Instance.InitLevel(FilthyModel.Instance.CurrentLevelID());

            UIHomeMainController.HideUI(false, true);
            UIManager.Instance.SetCanvasGroupAlpha(false, false);
            var guideWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGuidePortrait);
            if (guideWindow)
            {
                guideWindow.canvasGroup.alpha = 1f;
            }

            DynamicMapManager.Instance.PauseLogic = true;
            DecoManager.Instance.CurrentWorld.HideByPosition();
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
            _cameraSystem.Release();
            _cameraSystem = null;

            UIHomeMainController.ShowUI(false, true);

            PlayerManager.Instance.RecoverPlayer();
            DynamicMapManager.Instance.PauseLogic = false;
            DecoManager.Instance.CurrentWorld.ShowByPosition();
            
            XUtility.WaitSeconds(0.1f, () =>
            {
                AudioManager.Instance.PlayMusic(1, true);
            });
            
            FilthyModel.Instance.Release();

            UIManager.Instance.SetCanvasGroupAlpha(true, false);
            Resources.UnloadUnusedAssets();

            UIManager.Instance.EnableGraphicRaycaster(true);
       
            // if (!GuideSubSystem.Instance.isFinished(101))
            // {
            //     StorySubSystem.Instance.Trigger(StoryTrigger.Asmr, "1", b =>
            //     {
            //         GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
            //     });
            // }

            GuideSubSystem.Instance.CloseCurrent();
            
            _action?.Invoke();
        }
    }
}