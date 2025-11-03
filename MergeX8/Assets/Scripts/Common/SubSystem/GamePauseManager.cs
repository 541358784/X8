using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK;
using Framework;
using Gameplay;
using DragonPlus;

public class GamePauseManager : GlobalSystem<GamePauseManager>, IOnApplicationPause
{
    public enum PauseReasonMask : byte
    {
        BindFacebook = 1 << 0,
        LikeUs = 1 << 1,
        RateUs = 1 << 2,
        Pay = 1 << 3,
        Share = 1 << 4,
        ShowOfferWall = 1 << 5,
    }

    private byte _pauseReason;

    private Action _pauseCallback;

    public void AddCallback(Action callback)
    {
        _pauseCallback += callback;
    }

    public void RegisterPauseReason(PauseReasonMask mask)
    {
        _pauseReason |= (byte) mask;
    }

    private float m_PauseTime;

    public void OnApplicationPause(bool pause)
    {
        DebugUtil.Log($"OnApplicationPause, pause = {pause}");
        if (!pause)
        {
            float delta = Time.realtimeSinceStartup - m_PauseTime;
            Debug.Log($"pause time : {delta},  pause reason = {_pauseReason}");

            if (_pauseCallback != null)
            {
                _pauseCallback.Invoke();
                _pauseCallback = null;
            }

            _OnResumeFromBackground();
            _pauseReason = 0;
            DragonPlus.AudioManager.Instance.ResumeAllMusic();
        }
        else
        {
            m_PauseTime = Time.realtimeSinceStartup;
            DragonPlus.AudioManager.Instance.PauseAllMusic();
        }
    }


    public void _OnResumeFromBackground()
    {
        // if (!GuideSubSystem.Instance.IsShowingGuide())
        // {
        //     AdLogicManager.Instance.SpendOfferwallCurrency();
        //     if ((_pauseReason & (byte)(PauseReasonMask.Pay | PauseReasonMask.ShowOfferWall)) == 0)
        //     {
        //         AdLogicManager.Instance.TryShowInterstitial(InterstitialEnumeration.E_Background);
        //     }
        // }
    }
}