using System;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Newtonsoft.Json;

public static class LeaderBoardModel
{
    public enum RequestType
    {
        CreateOrGet,
        Update,
        Logout,
    }

    /// <summary>
    /// 请求服务器
    /// </summary>
    /// <param name="requestType">请求类型，创建获取，刷新，退出</param>
    /// <param name="type">，排行榜类型</param>
    /// <param name="uniqueArgs">排行榜标识参数</param>
    /// <param name="gloablData">透传参数 全局</param>
    /// <param name="callback">回调</param>
    /// <param name="extraInfo">额外信息（比如昵称头像，自定义信息后序列化为json字符串）</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void Request(RequestType requestType, string type, IDictionary<string, string> uniqueArgs, string gloablData,
        Action<bool, RequestType, LeaderboardListItem> callback,
        long score = 0,
        string extraInfo = "",uint leadBoardLength = 50)
    {
        //DebugUtil.LogError($"RequestType:{requestType}");
        switch (requestType)
        {
            case RequestType.CreateOrGet:
                RequestByCreateOrGet(type, uniqueArgs, gloablData, callback, score, extraInfo,leadBoardLength);
                break;
            case RequestType.Update:
                RequestByUpdate(type, uniqueArgs, gloablData, callback, score, extraInfo);
                break;
            case RequestType.Logout:
                RequestByLogout(type, callback);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
        }
    }

    /// <summary>
    /// 请求服务器创建或者获取排行榜数据
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <param name="uniqueArgs">排行榜参数</param>
    /// <param name="gloablData">透传参数 全局</param>
    /// <param name="callback">回调</param>
    /// <param name="extraInfo">额外信息（比如昵称头像，自定义信息后序列化为json字符串）</param>
    private static void RequestByCreateOrGet(string type, IDictionary<string, string> uniqueArgs, string gloablData,
        Action<bool, RequestType, LeaderboardListItem> callback,
        long score = 0,
        string extraInfo = "",uint leadBoardLength = 50)
    {
        var csdata = new CListMultipleLeaderboards();
        var item = new LeaderboardQueryItem()
        {
            LeaderboardType = type,
            UpdateData = new LeaderboardData()
            {
                LeaderboardType = type,
                Score = $"{score}",
                Extra = extraInfo,
                GlobalExtra = gloablData,
            },
            Count = leadBoardLength
        };
        DebugUtil.Log($"GlobalExtra:{gloablData}");
        DebugUtil.Log($"uniqueArgs:{JsonConvert.SerializeObject(uniqueArgs)}");
        item.UpdateData.UniqueArgs.Add(uniqueArgs);
        csdata.QueryList.Add(item);
        APIManager.Instance.Send(csdata, (SListMultipleLeaderboards result) =>
            {
                result.Leaderboards.TryGetValue(type, out var leaderboard);
                callback?.Invoke(true, RequestType.CreateOrGet, leaderboard);
            },
            (errorCode, errorMsg, response) =>
            {
                callback?.Invoke(false, RequestType.CreateOrGet, null);
                DebugUtil.LogError("### SListMultipleLeaderboards response Error ###" + errorMsg);
            });
    }

    /// <summary>
    /// 请求服务器更新排行榜数据
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <param name="uniqueArgs">排行榜参数</param>
    /// <param name="gloablData">透传参数 全局</param>
    /// <param name="callback">回调</param>
    /// <param name="extraInfo">额外信息（比如昵称头像，自定义信息后序列化为json字符串）</param>
    private static void RequestByUpdate(string type, IDictionary<string, string> uniqueArgs, string gloablData,
        Action<bool, RequestType, LeaderboardListItem> callback, long score = 0,
        string extraInfo = "")
    {
        var csdata = new CUpdateMultipleLeaderboards();
        csdata.DataList.Add(new LeaderboardData()
        {
            LeaderboardType = type,
            Score = $"{score}",
            UniqueArgs = {uniqueArgs},
            Extra = extraInfo,
            GlobalExtra = gloablData,
        });
        APIManager.Instance.Send(csdata,
            (SUpdateMultipleLeaderboards result) => { callback?.Invoke(true, RequestType.Update, null); },
            (errorCode, errorMsg, response) =>
            {
                callback?.Invoke(false, RequestType.Update, null);
                DebugUtil.LogError("### SUpdateMultipleLeaderboards response Error ###" + errorMsg);
            });
    }

    /// <summary>
    /// 请求服务器退出排行榜
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <param name="callback">回调</param>
    private static void RequestByLogout(string type, Action<bool, RequestType, LeaderboardListItem> callback)
    {
        var csdata = new CQuitMultipleLeaderboards();
        csdata.LeaderboardTypeList.Add(type);
        APIManager.Instance.Send(csdata,
            (SQuitMultipleLeaderboards result) => { callback?.Invoke(true, RequestType.Logout, null); },
            (errorCode, errorMsg, response) =>
            {
                callback?.Invoke(false, RequestType.Logout, null);
                DebugUtil.LogError("### SQuitMultipleLeaderboards response Error ###" + errorMsg);
            });
    }
}