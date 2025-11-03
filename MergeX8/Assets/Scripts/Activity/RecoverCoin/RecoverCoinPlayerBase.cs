using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json;
using UnityEngine;

public class RecoverCoinPlayerSortController
{
    private StorageRecoverCoinWeek _storageWeek;
    public StorageRecoverCoinWeek StorageWeek => _storageWeek;
    private RecoverCoinPlayerMe _me;
    public RecoverCoinPlayerMe Me => _me;
    public int MyRank => _me.Rank;

    public RecoverCoinPlayerSortController(StorageRecoverCoinWeek storageWeek)
    {
        _storageWeek = storageWeek;
        _players = new List<RecoverCoinPlayer>(){null};
        _playersById = new Dictionary<ulong, RecoverCoinPlayerOtherRealPeople>();
        if (_storageWeek.RobotList != null && _storageWeek.RobotList.Count > 0)
        {
            foreach (var robotConfig in _storageWeek.RobotList)
            {
                var robot = new RecoverCoinPlayerRobot(this, robotConfig);
                _players.Add(robot);
                robot.Rank = PlayerCount;
                robot.RefreshValue();
            }
        }
        // else if(_storageWeek.PlayerList != null && _storageWeek.PlayerList.Count > 0)
        // {
        //     foreach (var playerData in _storageWeek.PlayerList)
        //     {
        //         var otherPlayer = new RecoverCoinPlayerOtherRealPeople(this, playerData.Value);
        //         _players.Add(otherPlayer);
        //         otherPlayer.Rank = PlayerCount;
        //         otherPlayer.RefreshValue();
        //         _playersById.Add(playerData.Value.PlayerId,otherPlayer);
        //     }
        // }

        _me = new RecoverCoinPlayerMe(this, _storageWeek);
        _players.Add(_me);
        _me.Rank = PlayerCount;
        _me.RefreshValue();
        
    }

    public void AddNewPlayer(RecoverCoinPlayerServerStruct newPlayer)
    {
        if (_playersById.ContainsKey(newPlayer.PlayerId))
        {
            _playersById[newPlayer.PlayerId].UpdatePlayerData(newPlayer);
            return;
        }
        var otherPlayer = new RecoverCoinPlayerOtherRealPeople(this, newPlayer);
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
    public List<RecoverCoinPlayer> tempPlayerList = new List<RecoverCoinPlayer>();
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

    private Dictionary<ulong, RecoverCoinPlayerOtherRealPeople> _playersById;
    public int PlayerCount => Players.Count - 1;
    public List<RecoverCoinPlayer> Players => _players;
    private List<RecoverCoinPlayer> _players;

    public bool Sort(RecoverCoinPlayer changeValuePlayer, bool upper)
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

    public void Exchange(RecoverCoinPlayer changeValuePlayer1, RecoverCoinPlayer changeValuePlayer2)
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
    
    
    private List<Action<RecoverCoinPlayer>> _onRankChange;
    public void BindRankChangeAction(Action<RecoverCoinPlayer> onRankChange)
    {
        if (_onRankChange == null)
            _onRankChange = new List<Action<RecoverCoinPlayer>>();
        if (!_onRankChange.Contains(onRankChange))
            _onRankChange.Add(onRankChange);
    }

    public void UnBindRankChangeAction(Action<RecoverCoinPlayer> onRankChange)
    {
        _onRankChange.Remove(onRankChange);
    }

    public void OnRankChange(RecoverCoinPlayer rankChangePlayer)
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

public abstract class RecoverCoinPlayer
{
    public RecoverCoinPlayer(RecoverCoinPlayerSortController controller)
    {
        _controller = controller;
    }

    private RecoverCoinPlayer _previous;

    public void CleanPrevious()
    {
        _previous = null;
    }

    public RecoverCoinPlayer Previous
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

    public bool Higher(RecoverCoinPlayer target)
    {
        if (SortValue > target.SortValue)
            return true;
        if (SortValue == target.SortValue)
            return GetUpdateTime() < target.GetUpdateTime();
        return false;
    }

    private RecoverCoinPlayer _next;

    public void CleanNext()
    {
        _next = null;
    }

    public RecoverCoinPlayer Next
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
    protected RecoverCoinPlayerSortController _controller;
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

public class RecoverCoinPlayerRobot : RecoverCoinPlayer
{
    public override int Rank
    {
        get { return base.Rank; }
        set { base.Rank = value; }
    }
    
    public override string GetName()
    {
        return _robotConfig.PlayerName;
    }

    private StorageRecoverCoinRobot _robotConfig;
    private int MaxValue => _robotConfig.MaxStarCount;

    private int LastValue
    {
        get => _robotConfig.LastUpdateStarCount;
        set => _robotConfig.LastUpdateStarCount = value;
    }
    private int AddInterval {
        get => _robotConfig.NextUpdateInterval;
        set => _robotConfig.NextUpdateInterval = value;
    }
    private int NextUpdateValue => LastValue + AddInterval;

    public RecoverCoinPlayerRobot(RecoverCoinPlayerSortController controller, StorageRecoverCoinRobot robotConfig) :
        base(controller)
    {
        _robotConfig = robotConfig;
    }

    protected override int GetValue()
    {
        var curValue = Mathf.FloorToInt(_controller.TimeProgress * MaxValue);
        var loopCount = 0;
        while (curValue >= NextUpdateValue)
        {
            loopCount++;
            if (loopCount > 5)
            {
                LastValue = curValue;
                break;   
            }
            LastValue = NextUpdateValue;
            AddInterval = _controller.StorageWeek.GetRobotUpdateInterval();
        }
        return LastValue;
    }

    protected override ulong GetUpdateTime()
    {
        return ulong.MaxValue;
    }
}

public class RecoverCoinPlayerMe : RecoverCoinPlayer
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
    
    private StorageRecoverCoinWeek _storageWeek;

    public RecoverCoinPlayerMe(RecoverCoinPlayerSortController controller, StorageRecoverCoinWeek storageWeek) :
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

public class RecoverCoinPlayerOtherRealPeople : RecoverCoinPlayer
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

    public void UpdatePlayerData(RecoverCoinPlayerServerStruct storagePlayer)
    {
        _storagePlayer.SetValue(storagePlayer);
    }
    
    private RecoverCoinPlayerServerStruct _storagePlayer;

    public RecoverCoinPlayerOtherRealPeople(RecoverCoinPlayerSortController controller, RecoverCoinPlayerServerStruct storagePlayer) :
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