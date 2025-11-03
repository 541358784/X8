using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using UnityEngine;

public class ThemeDecorationLeaderBoardPlayerSortController
{
    public StorageThemeDecorationLeaderBoard _storageWeek;
    public StorageThemeDecorationLeaderBoard StorageWeek => _storageWeek;
    private ThemeDecorationLeaderBoardPlayerMe _me;
    public ThemeDecorationLeaderBoardPlayerMe Me => _me;
    public int MyRank => _me.Rank;

    public ThemeDecorationLeaderBoardPlayerSortController(StorageThemeDecorationLeaderBoard storageWeek)
    {
        _storageWeek = storageWeek;
        Players = new Dictionary<int, ThemeDecorationLeaderBoardPlayer>();
        
        _me = new ThemeDecorationLeaderBoardPlayerMe(this);
    }

    public void UpdateAllPlayerState(
        RepeatedField<global::DragonU3DSDK.Network.API.Protocol.LeaderboardEntry> leadBoardEntryList)
    {
        foreach (var leadBoardEntry in leadBoardEntryList)
        {
            var rank = (int) leadBoardEntry.Rank;
            if (!Players.TryGetValue(rank,out var player))
            {
                player = new ThemeDecorationLeaderBoardPlayerOtherRealPeople(this);
                Players.Add(rank,player);
            }
            player.UpdateStateWithServerData(leadBoardEntry);
        }
    }
    public int PlayerCount => Players.Count;
    // public List<ThemeDecorationLeaderBoardPlayer> tempPlayerList = new List<ThemeDecorationLeaderBoardPlayer>();
    // public void UpdateAll()
    // {
    //     tempPlayerList.Clear();
    //     foreach (var player in Players)
    //     {
    //         tempPlayerList.Add(player);
    //     }
    //     foreach (var player in tempPlayerList)
    //     {
    //         if (player != null)
    //         {
    //             player.RefreshValue();   
    //         }
    //     }
    // }

    public void UpdateMe(DragonU3DSDK.Network.API.Protocol.LeaderboardEntry myInfo)
    {
        _me.UpdateStateWithServerData(myInfo);
    }
    public Dictionary<int,ThemeDecorationLeaderBoardPlayer> Players;
    private List<Action<ThemeDecorationLeaderBoardPlayer>> _onRankChange;
    public void BindRankChangeAction(Action<ThemeDecorationLeaderBoardPlayer> onRankChange)
    {
        if (_onRankChange == null)
            _onRankChange = new List<Action<ThemeDecorationLeaderBoardPlayer>>();
        if (!_onRankChange.Contains(onRankChange))
            _onRankChange.Add(onRankChange);
    }

    public void UnBindRankChangeAction(Action<ThemeDecorationLeaderBoardPlayer> onRankChange)
    {
        _onRankChange.Remove(onRankChange);
    }

    public void OnRankChange(ThemeDecorationLeaderBoardPlayer rankChangePlayer)
    {
        if (_onRankChange != null)
        {
            foreach (var handler in _onRankChange)
            {
                handler(rankChangePlayer);
            } 
        }
    }
}

public abstract class ThemeDecorationLeaderBoardPlayer
{
    public ThemeDecorationLeaderBoardPlayer(ThemeDecorationLeaderBoardPlayerSortController controller)
    {
        _controller = controller;
    }

    public abstract void UpdateStateWithServerData(DragonU3DSDK.Network.API.Protocol.LeaderboardEntry serverData);
    public ThemeDecorationLeaderBoardPlayerSortController _controller;
    protected abstract ThemeDecorationLeaderBoardPlayerServerStruct GetValue();
    protected abstract ulong GetUpdateTime();
    
    public virtual int Rank
    {
        get;
        set;
    }

    
    public ThemeDecorationLeaderBoardPlayerServerStruct CurrentStruct;
    private int CurRank;
    public void RefreshValue()
    {
        var curValue = GetValue();
        if (CurrentStruct == null || !CurrentStruct.SameAs(curValue))
        {
            CurrentStruct = curValue.Copy();
            CurRank = Rank;
            _controller.OnRankChange(this);
        }
        else if (Rank != CurRank)
        {
            CurRank = Rank;
            _controller.OnRankChange(this);
        }
    }

    public List<ResData> Rewards => _controller.StorageWeek.RewardConfig().GetRewardsByRank(Rank);
    public virtual bool IsMe => false;
    public abstract AvatarViewState GetAvatarViewState();
    // public abstract Sprite GetHeadIcon();
    // public abstract Sprite GetHeadFrameIcon();
    // public abstract string GetName();
    
}

public class ThemeDecorationLeaderBoardPlayerMe : ThemeDecorationLeaderBoardPlayer
{
    public override AvatarViewState GetAvatarViewState()
    {
        return HeadIconUtils.GetMyViewState();
    }

    private ThemeDecorationLeaderBoardPlayerServerStruct _storagePlayer;
    public ThemeDecorationLeaderBoardPlayerMe(ThemeDecorationLeaderBoardPlayerSortController controller) :
        base(controller)
    {
        _storagePlayer = new ThemeDecorationLeaderBoardPlayerServerStruct()
        {
            StarCount=controller.StorageWeek.StarCount,
            StarUpdateTime = controller.StorageWeek.StarUpdateTime,
            PlayerId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId,
            PlayerName = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName,
            AvatarIconId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId,
            AvatarIconFrameId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetUserAvatarFrame().id,
            ViewState = HeadIconUtils.GetMyViewState(),
        };
        Rank = 1;
        RefreshValue();
    }
    public override void UpdateStateWithServerData(LeaderboardEntry serverData)
    {
        _storagePlayer.SetValue(ThemeDecorationLeaderBoardPlayerServerStruct.HandleServerRobotData(serverData));
        Rank = (int) serverData.Rank;
        RefreshValue();
    }

    protected override ThemeDecorationLeaderBoardPlayerServerStruct GetValue()
    {
        return _storagePlayer;
    }
    protected override ulong GetUpdateTime()
    {
        return _storagePlayer.StarUpdateTime;
    }

    public override bool IsMe => true;
}

public class ThemeDecorationLeaderBoardPlayerOtherRealPeople : ThemeDecorationLeaderBoardPlayer
{
    public override AvatarViewState GetAvatarViewState()
    {
        return IsMe ? HeadIconUtils.GetMyViewState() : _storagePlayer.ViewState;
    }

    private ThemeDecorationLeaderBoardPlayerServerStruct _storagePlayer;

    public ThemeDecorationLeaderBoardPlayerOtherRealPeople(ThemeDecorationLeaderBoardPlayerSortController controller) :
        base(controller)
    {
        _storagePlayer = new ThemeDecorationLeaderBoardPlayerServerStruct();
    }
    public override void UpdateStateWithServerData(LeaderboardEntry serverData)
    {
        _storagePlayer.SetValue(ThemeDecorationLeaderBoardPlayerServerStruct.HandleServerRobotData(serverData));
        Rank = (int) serverData.Rank;
        RefreshValue();
    }

    protected override ThemeDecorationLeaderBoardPlayerServerStruct GetValue()
    {
        return _storagePlayer;
    }
    protected override ulong GetUpdateTime()
    {
        return _storagePlayer.StarUpdateTime;
    }
    public override bool IsMe => _storagePlayer.PlayerId == StorageManager.Instance.GetStorage<StorageCommon>().PlayerId;
}