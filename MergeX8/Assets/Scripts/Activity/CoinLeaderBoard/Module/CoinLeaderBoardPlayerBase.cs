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

public class CoinLeaderBoardPlayerSortController
{
    private StorageCoinLeaderBoardWeek _storageWeek;
    public StorageCoinLeaderBoardWeek StorageWeek => _storageWeek;
    private CoinLeaderBoardPlayerMe _me;
    public CoinLeaderBoardPlayerMe Me => _me;
    public int MyRank => _me.Rank;

    public CoinLeaderBoardPlayerSortController(StorageCoinLeaderBoardWeek storageWeek)
    {
        _storageWeek = storageWeek;
        _players = new List<CoinLeaderBoardPlayer>(){null};
        _playersById = new Dictionary<ulong, CoinLeaderBoardPlayerOtherRealPeople>();
        // else if(_storageWeek.PlayerList != null && _storageWeek.PlayerList.Count > 0)
        // {
        //     foreach (var playerData in _storageWeek.PlayerList)
        //     {
        //         var otherPlayer = new CoinLeaderBoardPlayerOtherRealPeople(this, playerData.Value);
        //         _players.Add(otherPlayer);
        //         otherPlayer.Rank = PlayerCount;
        //         otherPlayer.RefreshValue();
        //         _playersById.Add(playerData.Value.PlayerId,otherPlayer);
        //     }
        // }

        _me = new CoinLeaderBoardPlayerMe(this, _storageWeek);
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
            var playerData = JsonConvert.DeserializeObject<CoinLeaderBoardPlayerServerStruct>(leadBoardEntry.Extra);
            if (playerData.PlayerId == StorageManager.Instance.GetStorage<StorageCommon>().PlayerId)
                continue;
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
                    if (_players[i].GetType() == typeof(CoinLeaderBoardPlayerOtherRealPeople))
                    {
                        var realPeoplePlayer = _players[i] as CoinLeaderBoardPlayerOtherRealPeople;
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
    }
    public void AddNewPlayer(CoinLeaderBoardPlayerServerStruct newPlayer)
    {
        if (_playersById.ContainsKey(newPlayer.PlayerId))
        {
            _playersById[newPlayer.PlayerId].UpdatePlayerData(newPlayer);
            return;
        }
        var otherPlayer = new CoinLeaderBoardPlayerOtherRealPeople(this, newPlayer);
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
    public List<CoinLeaderBoardPlayer> tempPlayerList = new List<CoinLeaderBoardPlayer>();
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

    private Dictionary<ulong, CoinLeaderBoardPlayerOtherRealPeople> _playersById;
    public int PlayerCount => Players.Count - 1;
    public List<CoinLeaderBoardPlayer> Players => _players;
    private List<CoinLeaderBoardPlayer> _players;

    public bool Sort(CoinLeaderBoardPlayer changeValuePlayer, bool upper)
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

    public void Exchange(CoinLeaderBoardPlayer changeValuePlayer1, CoinLeaderBoardPlayer changeValuePlayer2)
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
    
    
    private List<Action<CoinLeaderBoardPlayer>> _onRankChange;
    public void BindRankChangeAction(Action<CoinLeaderBoardPlayer> onRankChange)
    {
        if (_onRankChange == null)
            _onRankChange = new List<Action<CoinLeaderBoardPlayer>>();
        if (!_onRankChange.Contains(onRankChange))
            _onRankChange.Add(onRankChange);
    }

    public void UnBindRankChangeAction(Action<CoinLeaderBoardPlayer> onRankChange)
    {
        _onRankChange.Remove(onRankChange);
    }

    public void OnRankChange(CoinLeaderBoardPlayer rankChangePlayer)
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

public abstract class CoinLeaderBoardPlayer
{
    public CoinLeaderBoardPlayer(CoinLeaderBoardPlayerSortController controller)
    {
        _controller = controller;
    }

    private CoinLeaderBoardPlayer _previous;

    public void CleanPrevious()
    {
        _previous = null;
    }

    public CoinLeaderBoardPlayer Previous
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

    public bool Higher(CoinLeaderBoardPlayer target)
    {
        if (SortValue > target.SortValue)
            return true;
        if (SortValue == target.SortValue)
            return GetUpdateTime() < target.GetUpdateTime();
        return false;
    }

    private CoinLeaderBoardPlayer _next;

    public void CleanNext()
    {
        _next = null;
    }

    public CoinLeaderBoardPlayer Next
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
    protected CoinLeaderBoardPlayerSortController _controller;
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
    public abstract string GetName();
}

public class CoinLeaderBoardPlayerMe : CoinLeaderBoardPlayer
{
    public override int Rank
    {
        get { return base.Rank; }
        set { base.Rank = value; }
    }
    public override string GetName()
    {
        return StorageManager.Instance.GetStorage<StorageHome>().AvatarData.UserName;
    }
    
    private StorageCoinLeaderBoardWeek _storageWeek;

    public CoinLeaderBoardPlayerMe(CoinLeaderBoardPlayerSortController controller, StorageCoinLeaderBoardWeek storageWeek) :
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

public class CoinLeaderBoardPlayerOtherRealPeople : CoinLeaderBoardPlayer
{
    public override int Rank
    {
        get { return base.Rank; }
        set { base.Rank = value; }
    }
    public override string GetName()
    {
        return _storagePlayer.PlayerName;
    }

    public void UpdatePlayerData(CoinLeaderBoardPlayerServerStruct storagePlayer)
    {
        _storagePlayer.SetValue(storagePlayer);
    }
    
    public CoinLeaderBoardPlayerServerStruct _storagePlayer;

    public CoinLeaderBoardPlayerOtherRealPeople(CoinLeaderBoardPlayerSortController controller, CoinLeaderBoardPlayerServerStruct storagePlayer) :
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