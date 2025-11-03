using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonPlus;
using DG.Tweening;
using Framework;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class SceneFsmHappyGoGame : IFsmState
{
    public StatusType Type => StatusType.HappyGoGame;
    public float SceneTotalElapseTime { get; private set; }
    private bool m_IsInited = false;
    private float m_GameSpeedRatio;

    public void Enter(params object[] objs)
    {
        CoroutineManager.Instance.StartCoroutine(EnterGameLogic());
    }

    private IEnumerator EnterGameLogic()
    {
        AudioManager.Instance.PlayMusic("bgm_valentine", true);
        yield return AutoPopupManager.AutoPopupManager.Instance.EnterHappyGoPopUIViewLogic();
     
    }
    public void Update(float deltaTime)
    {
    }
    public void LateUpdate(float deltaTime)
    {
        
    }

    public void Exit()
    {
        AudioManager.Instance.PlayMusic(1, true);
        DOTween.KillAll(true);
    }
}

