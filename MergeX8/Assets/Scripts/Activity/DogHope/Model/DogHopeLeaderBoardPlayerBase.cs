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

public class DogHopeLeaderBoardPlayerSortController
{
    public StorageDogHopeLeaderBoard _storageWeek;
    public StorageDogHopeLeaderBoard StorageWeek => _storageWeek;
    private DogHopeLeaderBoardPlayerMe _me;
    public DogHopeLeaderBoardPlayerMe Me => _me;
    public int MyRank => _me.Rank;

    public DogHopeLeaderBoardPlayerSortController(StorageDogHopeLeaderBoard storageWeek)
    {
        _storageWeek = storageWeek;
        Players = new Dictionary<int, DogHopeLeaderBoardPlayer>();
        
        _me = new DogHopeLeaderBoardPlayerMe(this);
    }

    public void UpdateAllPlayerState(
        RepeatedField<global::DragonU3DSDK.Network.API.Protocol.LeaderboardEntry> leadBoardEntryList)
    {
        foreach (var leadBoardEntry in leadBoardEntryList)
        {
            var rank = (int) leadBoardEntry.Rank;
            if (!Players.TryGetValue(rank,out var player))
            {
                player = new DogHopeLeaderBoardPlayerOtherRealPeople(this);
                Players.Add(rank,player);
            }
            player.UpdateStateWithServerData(leadBoardEntry);
        }
    }
    public int PlayerCount => Players.Count;
    // public List<DogHopeLeaderBoardPlayer> tempPlayerList = new List<DogHopeLeaderBoardPlayer>();
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
    public Dictionary<int,DogHopeLeaderBoardPlayer> Players;
    private List<Action<DogHopeLeaderBoardPlayer>> _onRankChange;
    public void BindRankChangeAction(Action<DogHopeLeaderBoardPlayer> onRankChange)
    {
        if (_onRankChange == null)
            _onRankChange = new List<Action<DogHopeLeaderBoardPlayer>>();
        if (!_onRankChange.Contains(onRankChange))
            _onRankChange.Add(onRankChange);
    }

    public void UnBindRankChangeAction(Action<DogHopeLeaderBoardPlayer> onRankChange)
    {
        _onRankChange.Remove(onRankChange);
    }

    public void OnRankChange(DogHopeLeaderBoardPlayer rankChangePlayer)
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

public abstract class DogHopeLeaderBoardPlayer
{
    public DogHopeLeaderBoardPlayer(DogHopeLeaderBoardPlayerSortController controller)
    {
        _controller = controller;
    }

    public abstract void UpdateStateWithServerData(DragonU3DSDK.Network.API.Protocol.LeaderboardEntry serverData);
    public DogHopeLeaderBoardPlayerSortController _controller;
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
    // public abstract Sprite GetHeadIcon();
    // public abstract Sprite GetHeadFrameIcon();
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

public class DogHopeLeaderBoardPlayerMe : DogHopeLeaderBoardPlayer
{ 
    // public override Sprite GetHeadIcon()
    // {
    //     int iconId = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId;
    //     iconId = Math.Max(iconId, 0);
    //     TableAvatar avatar = GlobalConfigManager.Instance.GetTableAvatar(iconId);
    //     if(avatar == null)
    //         return null;
    //     return ResourcesManager.Instance.GetSpriteVariant(avatar.headIconAtlas, avatar.headIconName);
    // }
    // public override Sprite GetHeadFrameIcon()
    // {
    //     var avatarFrame = StorageManager.Instance.GetStorage<StorageHome>().AvatarData.GetUserAvatarFrame();
    //     if(avatarFrame == null)
    //         return null;
    //     return ResourcesManager.Instance.GetSpriteVariant(avatarFrame.headIconFrameAtlas, avatarFrame.headIconFrameName);
    // }
    public override AvatarViewState GetAvatarViewState()
    {
        return HeadIconUtils.GetMyViewState();
    }
    public override string GetName()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName;
    }

    private CommonLeaderBoardPlayerServerStruct _storagePlayer;
    public DogHopeLeaderBoardPlayerMe(DogHopeLeaderBoardPlayerSortController controller) :
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

public class DogHopeLeaderBoardPlayerOtherRealPeople : DogHopeLeaderBoardPlayer
{
    public override AvatarViewState GetAvatarViewState()
    {
        return IsMe?HeadIconUtils.GetMyViewState():_storagePlayer.ViewState;
    }
    // public override Sprite GetHeadIcon()
    // {
    //     int iconId = IsMe?StorageManager.Instance.GetStorage<StorageHome>().AvatarData.AvatarIconId:_storagePlayer.AvatarIconId;
    //     iconId = Math.Max(iconId, 0);
    //     TableAvatar avatar = GlobalConfigManager.Instance.GetTableAvatar(iconId);
    //     if(avatar == null)
    //         return null;
    //     return ResourcesManager.Instance.GetSpriteVariant(avatar.headIconAtlas, avatar.headIconName);
    // }
    // public override Sprite GetHeadFrameIcon()
    // {
    //     int iconId = _storagePlayer.AvatarIconFrameId;
    //     iconId = Math.Max(iconId, 0);
    //     GlobalConfigManager.Instance.TableAvatarFrames.TryGetValue(iconId,out var avatarFrame);
    //     if(avatarFrame == null)
    //         return null;
    //     return ResourcesManager.Instance.GetSpriteVariant(avatarFrame.headIconFrameAtlas, avatarFrame.headIconFrameName);
    // }
    public override string GetName()
    {
        return IsMe?StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName:_storagePlayer.PlayerName;
    }

    private CommonLeaderBoardPlayerServerStruct _storagePlayer;

    public DogHopeLeaderBoardPlayerOtherRealPeople(DogHopeLeaderBoardPlayerSortController controller) :
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