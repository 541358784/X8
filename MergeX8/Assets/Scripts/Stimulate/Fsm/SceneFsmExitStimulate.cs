using Decoration;
using DragonPlus;
using UnityEngine;

namespace Stimulate.Fsm
{
    public class SceneFsmExitStimulate : IFsmState
    {
        public StatusType Type => StatusType.Home;

        public void Enter(params object[] objs)
        {
            Input.multiTouchEnabled = true;
            if(!StorySubSystem.Instance.IsShowing)
                UIManager.Instance.SetCanvasGroupAlpha(true, false);
        
            BackHomeControl.CheckMainGuide(false, false);
        
            PlayerManager.Instance.RecoverPlayer();
        }
    
        public void Update(float deltaTime)
        {
            DecoManager.Instance?.Update(deltaTime);
        }
        public void LateUpdate(float deltaTime)
        {
        }
    
        public void Exit()
        {
         
        }
    }
}