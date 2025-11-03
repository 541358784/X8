using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using UnityEngine;

public class Easter2024LeaderBoardPlayerSortController
{
    private StorageEaster2024LeaderBoard _storageWeek;
    public StorageEaster2024LeaderBoard StorageWeek => _storageWeek;
    private Easter2024LeaderBoardPlayerMe _me;
    public Easter2024LeaderBoardPlayerMe Me => _me;
    public int MyRank => _me.Rank;

    public Easter2024LeaderBoardPlayerSortController(StorageEaster2024LeaderBoard storageWeek)
    {
        _storageWeek = storageWeek;
        _players = new List<Easter2024LeaderBoardPlayer>(){null};
        _playersById = new Dictionary<ulong, Easter2024LeaderBoardPlayerOtherRealPeople>();
        // else if(_storageWeek.PlayerList != null && _storageWeek.PlayerList.Count > 0)
        // {
        //     foreach (var playerData in _storageWeek.PlayerList)
        //     {
        //         var otherPlayer = new Easter2024LeaderBoardPlayerOtherRealPeople(this, playerData.Value);
        //         _players.Add(otherPlayer);
        //         otherPlayer.Rank = PlayerCount;
        //         otherPlayer.RefreshValue();
        //         _playersById.Add(playerData.Value.PlayerId,otherPlayer);
        //     }
        // }

        _me = new Easter2024LeaderBoardPlayerMe(this, _storageWeek);
        _players.Add(_me);
        _me.Rank = PlayerCount;
        _me.RefreshValue();
        
    }

    public void UpdateAllPlayerState(
        RepeatedField<global::DragonU3DSDK.Network.API.Protocol.LeaderboardEntry> leadBoardEntryList)
    {
        var playerIdKeyList = _playersById.Keys.ToList();
        foreach (var leadBoardEntry in leadBoardEntryList)
        {
            var playerData = JsonConvert.DeserializeObject<CommonLeaderBoardPlayerServerStruct>(leadBoardEntry.Extra);
            if (playerData.PlayerId == StorageManager.Instance.GetStorage<StorageCommon>().PlayerId)
                continue;
            if (playerData.ViewState == null)
                playerData.ViewState = new AvatarViewState(playerData.AvatarIconId, playerData.AvatarIconFrameId, playerData.PlayerName, true);
            AddNewPlayer(playerData);
            playerIdKeyList.Remove(playerData.PlayerId);
        }
        if (playerIdKeyList.Count > 0)//服务器的榜单和内存中的榜单不同,以服务器的榜单为准,删除掉本地内存中多余的玩家
        {
            foreach (var playerId in playerIdKeyList)
            {
                _playersById.Remove(playerId);
                for (var i = 0; i < _players.Count; i++)
                {
                    if (_players[i].GetType() == typeof(Easter2024LeaderBoardPlayerOtherRealPeople))
                    {
                        var realPeoplePlayer = _players[i] as Easter2024LeaderBoardPlayerOtherRealPeople;
                        if (realPeoplePlayer == null)
                            continue;
                        if (realPeoplePlayer._storagePlayer.PlayerId == playerId)
                        {
                            for (var j = i+1; j < _players.Count; j++)
                            {
                                _players[j].Rank--;
                            }
                            _players.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }
        OnRankChange(Me);
    }
    public void AddNewPlayer(CommonLeaderBoardPlayerServerStruct newPlayer)
    {
        if (_playersById.ContainsKey(newPlayer.PlayerId))
        {
            _playersById[newPlayer.PlayerId].UpdatePlayerData(newPlayer);
            return;
        }
        var otherPlayer = new Easter2024LeaderBoardPlayerOtherRealPeople(this, newPlayer);
        _players.Add(otherPlayer);
        otherPlayer.Rank = PlayerCount;
        otherPlayer.RefreshValue();
        _playersById.Add(newPlayer.PlayerId,otherPlayer);
    }

    public long CurTime
    {
        get
        {
            var serverTime = (long) APIManager.Instance.GetServerTime();
            if (serverTime < _storageWeek.StartTime)
                return _storageWeek.StartTime;
            if (serverTime > _storageWeek.EndTime)
                return _storageWeek.EndTime;
            return serverTime;
        }
    }

    private const long MaxTime = 7 * 24 * 3600 * 1000;
    public float TimeProgress => (CurTime - _storageWeek.StartTime) / (float) MaxTime;
    public List<Easter2024LeaderBoardPlayer> tempPlayerList = new List<Easter2024LeaderBoardPlayer>();
    public void UpdateAll()
    {
        tempPlayerList.Clear();
        foreach (var player in _players)
        {
            tempPlayerList.Add(player);
        }
        foreach (var player in tempPlayerList)
        {
            if (player != null)
            {
                player.RefreshValue();   
            }
        }
    }

    public void UpdateMe()
    {
        _me.RefreshValue();
    }

    private Dictionary<ulong, Easter2024LeaderBoardPlayerOtherRealPeople> _playersById;
    public int PlayerCount => Players.Count - 1;
    public List<Easter2024LeaderBoardPlayer> Players => _players;
    private List<Easter2024LeaderBoardPlayer> _players;

    public bool Sort(Easter2024LeaderBoardPlayer changeValuePlayer, bool upper)
    {
        Debug.Assert(_players[changeValuePlayer.Rank] == changeValuePlayer);
        var exchange = 0;
        if (upper)
        {
            while ((!changeValuePlayer.IsFirst) && changeValuePlayer.Higher(changeValuePlayer.Previous))
            {
                Exchange(changeValuePlayer, changeValuePlayer.Previous);
                exchange++;
                if (exchange > 50)
                {
                    Debug.LogError("循环超50次");
                    break;
                }
            }
        }
        else
        {
            while (!changeValuePlayer.IsLast && changeValuePlayer.Next.Higher(changeValuePlayer))
            {
                Exchange(changeValuePlayer, changeValuePlayer.Next);
                exchange++;
                if (exchange > 50)
                {
                    Debug.LogError("循环超50次");
                    break;
                }
            }
        }
        return exchange > 0;
    }

    public void Exchange(Easter2024LeaderBoardPlayer changeValuePlayer1, Easter2024LeaderBoardPlayer changeValuePlayer2)
    {
        var index1 = changeValuePlayer1.Rank;
        var index2 = changeValuePlayer2.Rank;
        changeValuePlayer1.CleanNext();
        changeValuePlayer1.CleanPrevious();
        changeValuePlayer2.CleanNext();
        changeValuePlayer2.CleanPrevious();
        changeValuePlayer1.Rank = index2;
        changeValuePlayer2.Rank = index1;
        _players[index1] = changeValuePlayer2;
        _players[index2] = changeValuePlayer1;
        var upper = changeValuePlayer1.Rank < changeValuePlayer2.Rank ? changeValuePlayer1 : changeValuePlayer2;
        var lower = changeValuePlayer1.Rank > changeValuePlayer2.Rank ? changeValuePlayer1 : changeValuePlayer2;
        upper.Previous?.CleanNext();
        lower.Next?.CleanPrevious();
    }
    
    
    private List<Action<Easter2024LeaderBoardPlayer>> _onRankChange;
    public void BindRankChangeAction(Action<Easter2024LeaderBoardPlayer> onRankChange)
    {
        if (_onRankChange == null)
            _onRankChange = new List<Action<Easter2024LeaderBoardPlayer>>();
        if (!_onRankChange.Contains(onRankChange))
            _onRankChange.Add(onRankChange);
    }

    public void UnBindRankChangeAction(Action<Easter2024LeaderBoardPlayer> onRankChange)
    {
        _onRankChange.Remove(onRankChange);
    }

    public void OnRankChange(Easter2024LeaderBoardPlayer rankChangePlayer)
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

public abstract class Easter2024LeaderBoardPlayer
{
    public Easter2024LeaderBoardPlayer(Easter2024LeaderBoardPlayerSortController controller)
    {
        _controller = controller;
    }

    private Easter2024LeaderBoardPlayer _previous;

    public void CleanPrevious()
    {
        _previous = null;
    }

    public Easter2024LeaderBoardPlayer Previous
    {
        get
        {
            if (_previous == null)
            {
                var previousIndex = Rank - 1;
                var players = _controller.Players;
                if (previousIndex <= 0 || previousIndex > _controller.PlayerCount)
                {
                    _previous = null;
                }
                else
                {
                    _previous = players[previousIndex];
                }
            }

            return _previous;
        }
    }

    public bool Higher(Easter2024LeaderBoardPlayer target)
    {
        if (SortValue > target.SortValue)
            return true;
        if (SortValue == target.SortValue)
            return GetUpdateTime() < target.GetUpdateTime();
        return false;
    }

    private Easter2024LeaderBoardPlayer _next;

    public void CleanNext()
    {
        _next = null;
    }

    public Easter2024LeaderBoardPlayer Next
    {
        get
        {
            if (_next == null)
            {
                var nextIndex = Rank + 1;
                var players = _controller.Players;
                if (nextIndex <= 0 || nextIndex > _controller.PlayerCount)
                {
                    _next = null;
                }
                else
                {
                    _next = players[nextIndex];
                }
            }

            return _next;
        }
    }

    public bool IsFirst => Previous == null;
    public bool IsLast => Next == null;
    protected Easter2024LeaderBoardPlayerSortController _controller;
    protected abstract int GetValue();
    protected abstract ulong GetUpdateTime();
    
    private int _rank;
    public virtual int Rank
    {
        get => _rank;
        set
        {
            _rank = value;
            _controller.OnRankChange(this);
        }
    }

    public int SortValue => _sortValue;
    private int _sortValue = int.MinValue;

    public void RefreshValue()
    {
        var curValue = GetValue();
        if (_sortValue != curValue)
        {
            var upper = curValue > _sortValue;
            _sortValue = curValue;
            var exchange = _controller.Sort(this, upper);
            if (!exchange)
            {
                _controller.OnRankChange(this);
            }
        }
    }

    public List<ResData> Rewards => _controller.StorageWeek.RewardConfig().GetRewardsByRank(Rank);
    public virtual bool IsMe => false;
    public virtual bool IsPeople => false;
    public abstract AvatarViewState GetAvatarViewState();
    public abstract string GetName();
}

public class Easter2024LeaderBoardPlayerMe : Easter2024LeaderBoardPlayer
{
    public override int Rank
    {
        get { return base.Rank; }
        set { base.Rank = value; }
    }
    public override AvatarViewState GetAvatarViewState()
    {
        return HeadIconUtils.GetMyViewState();
    }
    public override string GetName()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName;
    }
    
    private StorageEaster2024LeaderBoard _storageWeek;

    public Easter2024LeaderBoardPlayerMe(Easter2024LeaderBoardPlayerSortController controller, StorageEaster2024LeaderBoard storageWeek) :
        base(controller)
    {
        _storageWeek = storageWeek;
    }

    protected override int GetValue()
    {
        return _storageWeek.StarCount;
    }
    protected override ulong GetUpdateTime()
    {
        return _storageWeek.StarUpdateTime;
    }

    public override bool IsMe => true;
    public override bool IsPeople => true;
}

public class Easter2024LeaderBoardPlayerOtherRealPeople : Easter2024LeaderBoardPlayer
{
    public override int Rank
    {
        get { return base.Rank; }
        set { base.Rank = value; }
    }
    public override AvatarViewState GetAvatarViewState()
    {
        return IsMe?HeadIconUtils.GetMyViewState():_storagePlayer.ViewState;
    }
    public override string GetName()
    {
        return _storagePlayer.PlayerName;
    }

    public void UpdatePlayerData(CommonLeaderBoardPlayerServerStruct storagePlayer)
    {
        _storagePlayer.SetValue(storagePlayer);
    }
    
    public CommonLeaderBoardPlayerServerStruct _storagePlayer;

    public Easter2024LeaderBoardPlayerOtherRealPeople(Easter2024LeaderBoardPlayerSortController controller, CommonLeaderBoardPlayerServerStruct storagePlayer) :
        base(controller)
    {
        _storagePlayer = storagePlayer;
    }
    protected override int GetValue()
    {
        return _storagePlayer.StarCount;
    }
    protected override ulong GetUpdateTime()
    {
        return _storagePlayer.StarUpdateTime;
    }
    public override bool IsPeople => true;
}