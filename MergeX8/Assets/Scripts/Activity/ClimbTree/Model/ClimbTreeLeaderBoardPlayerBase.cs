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

public class ClimbTreeLeaderBoardPlayerSortController
{
    public StorageClimbTreeLeaderBoard _storageWeek;
    public StorageClimbTreeLeaderBoard StorageWeek => _storageWeek;
    private ClimbTreeLeaderBoardPlayerMe _me;
    public ClimbTreeLeaderBoardPlayerMe Me => _me;
    public int MyRank => _me.Rank;

    public ClimbTreeLeaderBoardPlayerSortController(StorageClimbTreeLeaderBoard storageWeek)
    {
        _storageWeek = storageWeek;
        Players = new Dictionary<int, ClimbTreeLeaderBoardPlayer>();
        
        _me = new ClimbTreeLeaderBoardPlayerMe(this);
    }

    public void UpdateAllPlayerState(
        RepeatedField<global::DragonU3DSDK.Network.API.Protocol.LeaderboardEntry> leadBoardEntryList)
    {
        foreach (var leadBoardEntry in leadBoardEntryList)
        {
            var rank = (int) leadBoardEntry.Rank;
            if (!Players.TryGetValue(rank,out var player))
            {
                player = new ClimbTreeLeaderBoardPlayerOtherRealPeople(this);
                Players.Add(rank,player);
            }
            player.UpdateStateWithServerData(leadBoardEntry);
        }
    }
    public int PlayerCount => Players.Count;
    // public List<ClimbTreeLeaderBoardPlayer> tempPlayerList = new List<ClimbTreeLeaderBoardPlayer>();
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
    public Dictionary<int,ClimbTreeLeaderBoardPlayer> Players;
    private List<Action<ClimbTreeLeaderBoardPlayer>> _onRankChange;
    public void BindRankChangeAction(Action<ClimbTreeLeaderBoardPlayer> onRankChange)
    {
        if (_onRankChange == null)
            _onRankChange = new List<Action<ClimbTreeLeaderBoardPlayer>>();
        if (!_onRankChange.Contains(onRankChange))
            _onRankChange.Add(onRankChange);
    }

    public void UnBindRankChangeAction(Action<ClimbTreeLeaderBoardPlayer> onRankChange)
    {
        _onRankChange.Remove(onRankChange);
    }

    public void OnRankChange(ClimbTreeLeaderBoardPlayer rankChangePlayer)
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

public abstract class ClimbTreeLeaderBoardPlayer
{
    public ClimbTreeLeaderBoardPlayer(ClimbTreeLeaderBoardPlayerSortController controller)
    {
        _controller = controller;
    }

    public abstract void UpdateStateWithServerData(DragonU3DSDK.Network.API.Protocol.LeaderboardEntry serverData);
    public ClimbTreeLeaderBoardPlayerSortController _controller;
    protected abstract CommonLeaderBoardPlayerServerStruct GetValue();
    protected abstract ulong GetUpdateTime();
    
    public virtual int Rank
    {
        get;
        set;
    }

    
    public CommonLeaderBoardPlayerServerStruct CurrentStruct;
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
    public abstract string GetName();

    public CommonLeaderBoardPlayerServerStruct HandleServerRobotData(LeaderboardEntry serverData)
    {
        var serverDataStruct =
            JsonConvert.DeserializeObject<CommonLeaderBoardPlayerServerStruct>(serverData.Extra);
        if (serverDataStruct.IsFake)
        {
            serverDataStruct.StarCount = (int)serverData.Score;
            serverDataStruct.StarUpdateTime = ulong.MinValue;
            serverDataStruct.PlayerId = serverData.LeaderboardEntryId.ToULong();
            serverDataStruct.PlayerName = "Robot"+serverData.LeaderboardEntryId;
            serverDataStruct.AvatarIconId = HeadIconUtils.GetRandomHead();
            serverDataStruct.AvatarIconFrameId = HeadIconUtils.GetRandomHeadFrame();
        }

        return serverDataStruct;
    }
}

public class ClimbTreeLeaderBoardPlayerMe : ClimbTreeLeaderBoardPlayer
{ 
    
    public override string GetName()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName;
    }
    public override AvatarViewState GetAvatarViewState()
    {
        return HeadIconUtils.GetMyViewState();
    }
    private CommonLeaderBoardPlayerServerStruct _storagePlayer;
    public ClimbTreeLeaderBoardPlayerMe(ClimbTreeLeaderBoardPlayerSortController controller) :
        base(controller)
    {
        _storagePlayer = new CommonLeaderBoardPlayerServerStruct()
        {
            StarCount=controller.StorageWeek.StarCount,
            StarUpdateTime = controller.StorageWeek.StarUpdateTime,
            PlayerId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId,
            PlayerName = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName,
            AvatarIconId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId,
            AvatarIconFrameId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetUserAvatarFrame().id,
        };
        Rank = 1;
        RefreshValue();
    }
    public override void UpdateStateWithServerData(LeaderboardEntry serverData)
    {
        _storagePlayer.SetValue(HandleServerRobotData(serverData));
        Rank = (int) serverData.Rank;
        RefreshValue();
    }

    protected override CommonLeaderBoardPlayerServerStruct GetValue()
    {
        return _storagePlayer;
    }
    protected override ulong GetUpdateTime()
    {
        return _storagePlayer.StarUpdateTime;
    }

    public override bool IsMe => true;
}

public class ClimbTreeLeaderBoardPlayerOtherRealPeople : ClimbTreeLeaderBoardPlayer
{
    public override AvatarViewState GetAvatarViewState()
    {
        return IsMe?HeadIconUtils.GetMyViewState():_storagePlayer.ViewState;
    }
    public override string GetName()
    {
        return IsMe?StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName:_storagePlayer.PlayerName;
    }

    private CommonLeaderBoardPlayerServerStruct _storagePlayer;

    public ClimbTreeLeaderBoardPlayerOtherRealPeople(ClimbTreeLeaderBoardPlayerSortController controller) :
        base(controller)
    {
        _storagePlayer = new CommonLeaderBoardPlayerServerStruct();
    }
    public override void UpdateStateWithServerData(LeaderboardEntry serverData)
    {
        _storagePlayer.SetValue(HandleServerRobotData(serverData));
        Rank = (int) serverData.Rank;
        RefreshValue();
    }

    protected override CommonLeaderBoardPlayerServerStruct GetValue()
    {
        return _storagePlayer;
    }
    protected override ulong GetUpdateTime()
    {
        return _storagePlayer.StarUpdateTime;
    }
    public override bool IsMe => _storagePlayer.PlayerId == StorageManager.Instance.GetStorage<StorageCommon>().PlayerId;
}