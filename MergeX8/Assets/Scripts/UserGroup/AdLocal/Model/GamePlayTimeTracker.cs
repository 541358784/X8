using UnityEngine;
using System;
using System.Collections.Generic;
using DragonU3DSDK;


public class GamePlayTimeData
{
    private float _sessionStartTime;
    private float _totalPlayTimeSeconds;
    private bool _isTracking = false;
    
    //特殊暂停,
    private bool _isSpecialPaused = false;
    
    public void StartTracking()
    {
        if (_isSpecialPaused)
            return;
        
        if (!_isTracking)
        {
            _sessionStartTime = Time.realtimeSinceStartup;
            _isTracking = true;
        }
    }

    public void StopTracking()
    {
        if (_isTracking)
        {
            // 计算本次会话时长并添加到总时长
            float sessionDuration = Time.realtimeSinceStartup - _sessionStartTime;
            _totalPlayTimeSeconds += sessionDuration;
            _isTracking = false;
        }
    }

    public void PauseTracking(bool isPaused)
    {
        _isSpecialPaused = isPaused;
        if (_isSpecialPaused)
            StopTracking();
        else
            StartTracking();
    }
    
    // 获取总游戏时长（秒）
    public float GetTotalPlayTime()
    {
        if (_isTracking)
        {
            // 如果正在计时，加上当前会话的时长
            float sessionDuration = Time.realtimeSinceStartup - _sessionStartTime;
            return _totalPlayTimeSeconds + sessionDuration;
        }
        return _totalPlayTimeSeconds;
    }
    
    public void ResetTracking()
    {
        _totalPlayTimeSeconds = 0f;
        _isTracking = false;
        StartTracking();
    }
    
}



public class GamePlayTimeTracker : Manager<GamePlayTimeTracker>
{
    
    private Dictionary<string, GamePlayTimeData> _dicGamePlayTimeData = new Dictionary<string, GamePlayTimeData>();


    /// <summary>
    /// 开始计时
    /// </summary>
    /// <param name="placeId"></param>
    /// <param name="isReset"></param>
    public void StartTracking(string placeId,bool isReset)
    {
        DebugUtil.LogError($" GamePlayTimeTracker =>开始记录净游戏时长  {placeId}   {isReset}");
        if (!_dicGamePlayTimeData.TryGetValue(placeId, out GamePlayTimeData data))
        {
            data = new GamePlayTimeData();
            _dicGamePlayTimeData.Add(placeId, data);
        }
        if (!isReset)
            data.StartTracking();
        else
            data.ResetTracking();
      
    }
    
    /// <summary>
    /// 暂停计时
    /// </summary>
    /// <param name="placeId"></param>
    /// <param name="isPaused"></param>
    public void PauseTracking(string placeId,bool isPaused)
    {
        if (_dicGamePlayTimeData.TryGetValue(placeId, out GamePlayTimeData data))
        {
            data.PauseTracking(isPaused);
        }
    }
    
    /// <summary>
    /// 停止计时
    /// </summary>
    /// <param name="placeId"></param>
    public void StopTracking(string placeId)
    {
        if (_dicGamePlayTimeData.TryGetValue(placeId, out GamePlayTimeData data))
        {
            data.StopTracking();
        }
    }

    //获取时长
    public float GetTotalPlayTime(string placeId)
    {
        if (_dicGamePlayTimeData.TryGetValue(placeId, out GamePlayTimeData data))
        {
            DebugUtil.LogError($"GamePlayTimeTracker =>获取净时长  {placeId}   {data.GetTotalPlayTime()}");
            return data.GetTotalPlayTime();
        }
        return 0f;
    }



    public void StartAllTracking()
    {
        foreach (var timeData in _dicGamePlayTimeData)
        {
            timeData.Value.StartTracking();
        }
    }
    
    public void StopAllTracking()
    {
        foreach (var timeData in _dicGamePlayTimeData)
        {
            timeData.Value.StopTracking();
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // 游戏进入后台时停止计时
            StopAllTracking();
        }
        else
        {
            // 游戏回到前台时继续计时
            StartAllTracking();
        }
    }
} 