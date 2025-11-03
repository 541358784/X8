using System;
using System.Reflection;
using DragonU3DSDK.Storage;
using Framework;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

public class AppIconChangerSystem : GlobalSystem<AppIconChangerSystem>, IInitable
{
    //全屏广告播放中
    public bool IsScreenAdPlaying { get; set; } = false;

    //苹果绑定
    public bool IsFBAppleBinding { get; set; } = false;

    //支付中
    public bool IsPaying { get; set; } = false;

    //跳转FB页面
    public bool IsSkipingFBPage { get; set; } = false;

    //跳转评价
    public bool IsSkipingAppraise { get; set; } = false;

    //显示隐私政策
    public bool IsShowingPrivacyPollcy { get; set; } = false;

    //显示服务
    public bool IsShowingService { get; set; } = false;

    //显示推送设置
    public bool IsShowingPushSetting { get; set; } = false;
    public bool IsUrlOpening { get; set; } = false;
    public bool IsShowingUmp { get; set; } = false;

    private bool _isLogin = false;
    
    public void Init()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.LOGIN_SUCCESS, OnLoginSuccess);
    }

    public void Release()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.LOGIN_SUCCESS, OnLoginSuccess);
    }
    
    private void OnLoginSuccess(BaseEvent obj)
    {
        _isLogin = true;
#if UNITY_IOS && !UNITY_EDITOR
        ChangeIcon();
#endif
        // Debug.LogError("OnLoginSuccess");
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if(!_isLogin)
            return;
        
#if UNITY_ANDROID && !UNITY_EDITOR
        if (pauseStatus)
        {
#if GOOGLE_UMP_ENABLE
            IsShowingUmp = (ConsentInformation.ConsentStatus == ConsentStatus.Required);
#endif
            ChangeIcon();
        }
        else
            ResetFlag();
#endif
    }

    private void ResetFlag()
    {
        IsScreenAdPlaying = false;
        IsFBAppleBinding = false;
        IsPaying = false;
        IsSkipingFBPage = false;
        IsSkipingAppraise = false;
        IsShowingPrivacyPollcy = false;
        IsShowingPushSetting = false;
        IsShowingService = false;
        IsUrlOpening = false;
        IsShowingUmp = false;
    }

    private bool CanChangeIcon()
    {
        bool ret = !IsScreenAdPlaying && !IsFBAppleBinding && !IsPaying && !IsSkipingFBPage && !IsSkipingAppraise &&
                   !IsShowingPrivacyPollcy && !IsShowingPushSetting && !IsShowingService && !IsUrlOpening &&
                   !IsShowingUmp;
        return ret;
    }

    //切换icon
    private void ChangeIcon()
    {
        if (!CanChangeIcon())
            return;
#if UNITY_IOS
    IconChanger.OnChangeIcon();
#endif
    }
}