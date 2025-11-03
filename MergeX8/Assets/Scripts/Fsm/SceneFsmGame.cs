using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonPlus;
using DG.Tweening;
using Framework;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class SceneFsmGame : IFsmState
{
    public StatusType Type => StatusType.Game;
    public float SceneTotalElapseTime { get; private set; }
    private bool m_IsInited = false;
    private float m_GameSpeedRatio;

    public void Enter(params object[] objs)
    {
        MergeGuideLogic.Instance.CheckMergeGuide();
        KeepPetModel.Instance.CheckDrumstickOrder();
        CoroutineManager.Instance.StartCoroutine(EnterGameLogic());
    }

    private IEnumerator EnterGameLogic()
    {
        yield return AutoPopupManager.AutoPopupManager.Instance.EnterGamePopUIViewLogic();
     
    }
    public void Update(float deltaTime)
    {
    }
    public void LateUpdate(float deltaTime)
    {
        
    }

    public void Exit()
    {
        DOTween.KillAll(true);
    }
}


/// <summary>
/// 进入game的参数
/// </summary>
public class EnterGameParm
{
    // 关卡id
    public int LevelID = -1;
}