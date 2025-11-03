using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using FishEatFishSpace;
using Gameplay;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using UnityEngine;

public class SeaRacingPlayerSortController
{
    private StorageSeaRacingRound _storageWeek;
    public StorageSeaRacingRound StorageWeek => _storageWeek;
    private SeaRacingPlayerMe _me;
    public SeaRacingPlayerMe Me => _me;
    public int MyRank => _me.Rank;

    public SeaRacingPlayerSortController(StorageSeaRacingRound storageWeek)
    {
        _storageWeek = storageWeek;
        _players = new List<SeaRacingPlayer>(){null};
       
        if (_storageWeek.RobotList != null && _storageWeek.RobotList.Count > 0)
        {
            foreach (var robotConfig in _storageWeek.RobotList)
            {
                var robot = new SeaRacingPlayerRobot(this, robotConfig);
                _players.Add(robot);
                robot.Rank = PlayerCount;
                
                if(storageWeek.IsStart)
                    robot.RefreshValue();
            }
        }

        _me = new SeaRacingPlayerMe(this, _storageWeek);
        _players.Add(_me);
        _me.Rank = PlayerCount;
        _me.RefreshValue();
        
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
    public long PassedTime => CurTime - _storageWeek.StartTime;
    public float PassedMin => PassedTime / (float)XUtility.Min;
    public List<SeaRacingPlayer> tempPlayerList = new List<SeaRacingPlayer>();
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
    public int PlayerCount => Players.Count - 1;
    public List<SeaRacingPlayer> Players => _players;
    private List<SeaRacingPlayer> _players;

    public bool Sort(SeaRacingPlayer changeValuePlayer, bool upper)
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

    public void Exchange(SeaRacingPlayer changeValuePlayer1, SeaRacingPlayer changeValuePlayer2)
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
    
    
    private List<Action<SeaRacingPlayer>> _onRankChange;
    public void BindRankChangeAction(Action<SeaRacingPlayer> onRankChange)
    {
        if (_onRankChange == null)
            _onRankChange = new List<Action<SeaRacingPlayer>>();
        if (!_onRankChange.Contains(onRankChange))
            _onRankChange.Add(onRankChange);
    }

    public void UnBindRankChangeAction(Action<SeaRacingPlayer> onRankChange)
    {
        _onRankChange.Remove(onRankChange);
    }

    public void OnRankChange(SeaRacingPlayer rankChangePlayer)
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

public abstract class SeaRacingPlayer
{
    public SeaRacingPlayer(SeaRacingPlayerSortController controller)
    {
        _controller = controller;
    }

    private SeaRacingPlayer _previous;

    public void CleanPrevious()
    {
        _previous = null;
    }

    public SeaRacingPlayer Previous
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

    public bool Higher(SeaRacingPlayer target)
    {
        if (SortValue > target.SortValue)
            return true;
        if (SortValue == target.SortValue)
            return GetUpdateTime() < target.GetUpdateTime();
        return false;
    }

    private SeaRacingPlayer _next;

    public void CleanNext()
    {
        _next = null;
    }

    public SeaRacingPlayer Next
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
    protected SeaRacingPlayerSortController _controller;
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
    public virtual bool IsMe => false;
    public virtual bool IsPeople => false;
    public abstract string GetName();
    public abstract AvatarViewState GetAvatarViewState();
}

public class SeaRacingPlayerMe : SeaRacingPlayer
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
    
    private StorageSeaRacingRound _storageWeek;

    public SeaRacingPlayerMe(SeaRacingPlayerSortController controller, StorageSeaRacingRound storageWeek) :
        base(controller)
    {
        _storageWeek = storageWeek;
    }

    protected override int GetValue()
    {
        return _storageWeek.Score;
    }
    protected override ulong GetUpdateTime()
    {
        return _storageWeek.ScoreUpdateTime;
    }

    public override bool IsMe => true;
    public override bool IsPeople => true;
}

public enum SeaRacingRobotType
{
    Normal = 1,
    Limit = 2,
}
public class SeaRacingPlayerRobot : SeaRacingPlayer
{
    public override int Rank
    {
        get { return base.Rank; }
        set { base.Rank = value; }
    }
    public override AvatarViewState GetAvatarViewState()
    {
        return new AvatarViewState(_robotConfig.AvatarIconId, -1, "Robot", false);
    }
    public override string GetName()
    {
        return _robotConfig.PlayerName;
    }

    private StorageSeaRacingRobot _robotConfig;
    public bool IsMaxScore => LastValue >= _controller.StorageWeek.MaxScore;
    private int LastValue
    {
        get => _robotConfig.Score;
        set => _robotConfig.Score = value;
    }

    public bool IsLimit => _robotConfig.RobotType == (int) SeaRacingRobotType.Limit;

    public SeaRacingPlayerRobot(SeaRacingPlayerSortController controller, StorageSeaRacingRobot robotConfig) :
        base(controller)
    {
        _robotConfig = robotConfig;
    }

    protected override int GetValue()
    {
        if (IsMaxScore)
            return LastValue;
        if (!_controller.StorageWeek.IsStart)
            return 0;
        var curTime = (long)APIManager.Instance.GetServerTime();
        while (curTime >= _robotConfig.UpdateScoreTime + _robotConfig.UpdateScoreInterval)
        {
            var curValue = LastValue + _robotConfig.UpdateScoreValue;
            if (IsLimit)
            {
                var limitValue = Mathf.FloorToInt(_controller.StorageWeek.Score * _robotConfig.ScoreLimit);
                if (curValue > limitValue)
                {
                    curValue = limitValue;
                }
            }
            if (curValue >= _controller.StorageWeek.MaxScore)
            {
                // Debug.LogError("机器人满分 随机组"+_robotConfig.RandomConfigId);
                LastValue = _controller.StorageWeek.MaxScore;
            }
            else
            {
                // Debug.LogError("机器人加分"+_robotConfig.UpdateScoreValue +" 随机组"+_robotConfig.RandomConfigId);
                LastValue = curValue;
            }
            _robotConfig.UpdateScoreCount++;
            _robotConfig.UpdateScoreTime += _robotConfig.UpdateScoreInterval;
            if (_robotConfig.UpdateScoreCount >= _robotConfig.UpdateScoreMaxCount)
            {
                _robotConfig.RandomRobotConfig();
            }
            else
            {
                _robotConfig.RandomRobotState();   
            }
            if (IsMaxScore)
                break;
        }
        return LastValue;
    }

    protected override ulong GetUpdateTime()
    {
        return (ulong)_robotConfig.UpdateScoreTime;
    }
}