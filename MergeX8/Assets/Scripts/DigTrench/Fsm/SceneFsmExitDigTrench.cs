using Decoration;
using DG.Tweening;
using DragonPlus;
using Makeover;
using UnityEngine;

public class SceneFsmExitDigTrench : IFsmState
{
    public StatusType Type => StatusType.Home;

    private MakeoverSystem makeoverSystem;
    
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