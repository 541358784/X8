using System.Net.NetworkInformation;
using Cysharp.Threading.Tasks;
using Decoration;
using Decoration.DynamicMap;
using Farm.Model;
using Screw.GameLogic;
using Screw.Module;
using Screw.UserData;
using UnityEngine;

namespace Screw
{
    public class SceneFsmScrewHome : IFsmState
    {
        public StatusType Type => StatusType.ScrewHome;
        public void Enter(params object[] objs)
        {
            FarmModel.Instance.AnimShow(false);
            UIHomeMainController.HideUI(false, true);
            PlayerManager.Instance.HidePlayer();
            DecoManager.Instance.CurrentWorld.HideByPosition();
            DynamicMapManager.Instance.PauseLogic = true;

            ResBarModule.Instance.Release();
            
            UIModule.Instance.ShowUI(typeof(UIScrewHomePopup));

            PlayRewardAnim(objs);
        }

        public void Update(float deltaTime)
        {
        }

        public void LateUpdate(float deltaTime)
        {
        }

        public void Exit()
        {
            DynamicMapManager.Instance.PauseLogic = false;
            UIModule.Instance.CloseWindow<UIScrewHomePopup>();
        }

        private async UniTask PlayRewardAnim(object[] objs)
        {
            if (objs == null || objs.Length == 0)
            {
                TriggerGuide();
                return;
            }

            await UniTask.WaitForEndOfFrame();
            
            UIRoot.Instance.EnableEventSystem = false;
            int resCount = (int)objs[0];
            EventDispatcher.Instance.DispatchEventImmediately(ConstEvent.SCREW_REFRESH_RES, ResType.Coin, UserData.UserData.Instance.GetRes(ResType.Coin)-resCount, resCount, false);
            await UniTask.WaitForSeconds(0.1f);
            
            FlyModule.Instance.Fly(1, resCount, Vector3.zero, endAction: () =>
            {
                UIRoot.Instance.EnableEventSystem = true;
                TriggerGuide();
            });
        }

        private void TriggerGuide()
        {
            if(GuideSubSystem.Instance.isFinished(101))
                return;
            if(GuideSubSystem.Instance.isFinished(4420))
                return;
            
            if(ScrewGameLogic.Instance.GetMainLevelIndex() < 11 && !EnergyData.Instance.IsEnergyEmpty())
                return;

            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ScrewToMerge, null);
        }
    }
}