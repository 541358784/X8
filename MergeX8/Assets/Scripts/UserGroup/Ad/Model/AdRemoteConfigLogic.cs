using System;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;

public class AdRemoteConfigLogic : Manager<AdRemoteConfigLogic>
{
    private int requestTime = 60;
    private int requestIndex = 1;
    private int requestMaxIndex = 5;

    private float curRequestTime = 0;

    /// <summary>
    /// 升级获取分层配置最大等级
    /// </summary>
    private int _getRemoteLevelMax = 5;
    
    
    /// <summary>
    /// 前5次1分钟间隔
    /// </summary>
    private int _firstRequestIndex = 1;
    private int _firstRequestMaxIndex = 5;
    
    public void Init()
    {
        requestIndex = 1;
        _firstRequestIndex = 1;
        curRequestTime = Time.realtimeSinceStartup;
        
        CancelInvoke(nameof(InvokeFunction));
        InvokeRepeating(nameof(InvokeFunction), requestTime, requestTime+1);
        
        EventDispatcher.Instance.AddEventListener(MergeEvent.DO_REFRESH_EXPERENCE, OnLevelUp);

    }

    private void InvokeFunction()
    {
        if(Time.realtimeSinceStartup - curRequestTime < requestIndex*requestTime)
            return;
        
        curRequestTime = Time.realtimeSinceStartup;
        if (_firstRequestIndex > _firstRequestMaxIndex)
            requestIndex++;
        _firstRequestIndex++;
        requestIndex = Math.Min(requestIndex, requestMaxIndex);
        AdConfigManager.Instance.InitServerConfig();
        AdLocalConfigHandle.Instance.InitServerConfig();
        Debug.LogError("====================================================> 定时刷新分层");
        DragonPlus.ConfigHub.ConfigHubManager.Instance.UpdateRemoteConfig(true);
    }

    private void OnLevelUp(BaseEvent e)
    {
        if (ExperenceModel.Instance.GetLevel()<=_getRemoteLevelMax)
        {
            Debug.LogError("====================================================> 升级刷新分层");
            DragonPlus.ConfigHub.ConfigHubManager.Instance.UpdateRemoteConfig(true);
        }
    }
    
}