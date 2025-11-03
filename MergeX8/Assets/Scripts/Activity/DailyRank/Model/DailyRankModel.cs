using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Framework;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class DailyRankModel : ActivityEntityBase
{
    private static DailyRankModel _instance;
    public static DailyRankModel Instance => _instance ?? (_instance = new DailyRankModel());
    

    public override string Guid => "OPS_EVENT_TYPE_DAILY_RANK";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }
    
    private readonly char[] RandomCodes ={'0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q',
        'r','s','t','u','v','w','x','y','z'};
    
    public StorageDailyRankGroup _dailyRankGroup
    {
        get
        {
            return StorageManager.Instance.GetStorage<StorageHome>().DailyRankGroup;
        }
    }
    public StorageDailyRank _curDailyRank
    {
        get
        {
            if (ActivityId == null)
                return null;

            if (_dailyRankGroup.DailyRanks.ContainsKey(StorageKey))
                return _dailyRankGroup.DailyRanks[StorageKey];

            return null;
        }
    }


    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        DailyRankConfigManager.Instance.InitFromServerData(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
    }
    
    public override void UpdateActivityState()
    {
    }

    protected override void InitServerDataFinish()
    {
        if (!_dailyRankGroup.DailyRanks.ContainsKey(StorageKey))
        {
            _dailyRankGroup.DailyRanks[StorageKey] = new StorageDailyRank();
            InitDifficulty();
            InitReward();
            InitBotScore();
        }

        InitRobots();
        _dailyRankGroup.DailyRanks[StorageKey].ActiveEndTime = EndTime;
        InitInvoke();
    }

    private void InitDifficulty()
    {
        _dailyRankGroup.DefaultValues.Clear();
        
        var values = AdConfigHandle.Instance.GetDailyRankDefaultValue();
        values.ForEach(a=>_dailyRankGroup.DefaultValues.Add(a));
        
        if (_dailyRankGroup.DifficultyId == 0)//首次开启
        {
            _dailyRankGroup.DifficultyId = DailyRankConfigManager.Instance.defaultData.id;
        }
        
        var difficulty = DailyRankConfigManager.Instance._difficultyData.Find(a => a.id == _dailyRankGroup.DifficultyId);
        if (difficulty == null)
        {
            _dailyRankGroup.DifficultyId = DailyRankConfigManager.Instance.defaultData.id;
            difficulty = DailyRankConfigManager.Instance.defaultData;
            _dailyRankGroup.DefaultValue = difficulty.defaultValue;
        }

        for (int i = 0; i < _dailyRankGroup.DefaultValues.Count; i++)
        {
            _dailyRankGroup.DefaultValues[i] = values[i]*difficulty.defaultValue/100; 
        }
        
        for (int i = 0; i < 2; i++)
        {
            int curValue = _dailyRankGroup.WinCount;
            int needValue = difficulty.winCount;
            int changeValue = difficulty.winChangeValue;
            if (i == 1)
            {
                curValue = _dailyRankGroup.LostCount;
                needValue = difficulty.loseCount;
                changeValue = difficulty.loseChangeValue;
            }
            
            int changeId = CalculateDifficulty(_dailyRankGroup.DifficultyId, curValue, needValue, changeValue);
            if (changeId < 0)
                continue;
            
            _dailyRankGroup.DifficultyId = changeId;
            _dailyRankGroup.WinCount = 0;
            _dailyRankGroup.LostCount = 0;
            return;
        }
    }

    private void InitReward()
    {
        foreach (var reward in DailyRankConfigManager.Instance._rewadData)
        {
            if (_curDailyRank.Rewards.Count < reward.rank)
            {
               _curDailyRank.Rewards.Add(new StorageDailyRankRewardGroup());
            }

            StorageDailyRankReward storageDaily = new StorageDailyRankReward();
            storageDaily.Type = reward.rewardType;
            storageDaily.Num = reward.rewardNum;
            _curDailyRank.Rewards[reward.rank-1].Rewards.Add(storageDaily);
        }
    }

    private void InitBotScore()
    {
        _dailyRankGroup.BotScore.Clear();

        foreach (var botscore in DailyRankConfigManager.Instance._botscoreData)
        {
            _dailyRankGroup.BotScore.Add(new StorageDailyRankBotscore());

            _dailyRankGroup.BotScore[_dailyRankGroup.BotScore.Count - 1].DefaultMin = botscore.defaultMin;
            _dailyRankGroup.BotScore[_dailyRankGroup.BotScore.Count - 1].DefaultMax = botscore.defaultMax;
            _dailyRankGroup.BotScore[_dailyRankGroup.BotScore.Count - 1].Le_Min_80 = botscore.le_Min_80;
            _dailyRankGroup.BotScore[_dailyRankGroup.BotScore.Count - 1].Le_Max_80 = botscore.le_Max_80;
            _dailyRankGroup.BotScore[_dailyRankGroup.BotScore.Count - 1].Gr_Min_100 = botscore.gr_Min_100;
            _dailyRankGroup.BotScore[_dailyRankGroup.BotScore.Count - 1].Gr_Max_100 = botscore.gr_Max_100;
        }
    }
    private void InitInvoke()
    {
        if(!IsOpenActivity())
            return;
        
        UpdateRobotInfo(false, null);
    }

    
    public void UpdateRobotInfo(bool mandatory, StorageDailyRank dailyRank)
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyRank))
            return;

        if (!mandatory && dailyRank == null)
        {
            if(!IsOpenActivity() || IsActiveTimeEnd(null))
                return;
        }

        dailyRank = dailyRank == null ? _curDailyRank : dailyRank;

        if (dailyRank.ActiveEndTime == dailyRank.UpdateTime)
        {
            //RootCheat(mandatory, dailyRank);
            return;
        }
        
        bool isTimeEnd = false;
        ulong curServerTime = APIManager.Instance.GetServerTime();
        curServerTime += 15 * 1000;
        if (dailyRank.ActiveEndTime < curServerTime)
        {
            isTimeEnd = true;
            curServerTime = dailyRank.ActiveEndTime;
        }
        
        if (curServerTime - dailyRank.UpdateTime > int.MaxValue)
        {
            dailyRank.UpdateTime = curServerTime-30 * 60*1000;
        }
        
        int difference = (int)(curServerTime - dailyRank.UpdateTime) / 1000;
      
        if (!mandatory && difference < 30 * 60)
            return;

        int diffRate = difference / (30 * 60);
        if (!mandatory && diffRate <= 0)
            return;

        if (mandatory)
            diffRate = diffRate == 0 ? 1 : diffRate;
                
        if(isTimeEnd)
            dailyRank.UpdateTime = curServerTime;
        
        for (int i = 0; i < dailyRank.Robots.Count; i++)
        {
            StorageDailyRankRobot robot = dailyRank.Robots[i];
            var botscore = _dailyRankGroup.BotScore[i];

            if (dailyRank.CurScore == 0)
            {
                int randomValue = Random.Range(botscore.DefaultMin, botscore.DefaultMax+1);
                int addScore = Mathf.CeilToInt(1.0f * _dailyRankGroup.DefaultValues[i] * (randomValue / 10000f) * diffRate);
                robot.CurScore += addScore;
            }
            else
            {
                int randomValue = 0;
                float rate = 1.0f * dailyRank.CurScore / _dailyRankGroup.DefaultValues[i];
                int score = 0;
                if (rate < 0.8f)
                {
                    randomValue =  Random.Range(botscore.Le_Min_80, botscore.Le_Max_80+1);
                    score =  (int)(1.0f * dailyRank.CurScore * (randomValue / 100f))+1;
                }
                else if (rate > 1.0f)
                {
                    randomValue =  Random.Range(botscore.Gr_Min_100, botscore.Gr_Max_100+1);
                    score =  (int)(1.0f * dailyRank.CurScore * (randomValue / 100f))+1;
                }
                else
                {
                    if (i == 0)
                    {
                        randomValue = Random.Range((int)(rate * 100), 100+1);
                        score =  (int)(1.0f * _dailyRankGroup.DefaultValues[i] * (randomValue / 100f))+1;
                    }
                }
                
                if (score > robot.CurScore)
                    robot.CurScore = score;
            }
        }
        dailyRank.UpdateTime = curServerTime;
        dailyRank.Robots.Sort((x,y)=>y.CurScore - x.CurScore);

        //RootCheat(false, dailyRank);
        EventDispatcher.Instance.DispatchEvent(EventEnum.DAILY_RANK_UPDATE);
    }
      
    public void UpdateActivity()
    {    
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyRank))
            return;
        
        if (_curDailyRank == null)
            return;
        
        if (!IsActiveTimeEnd(null))
            return;
        
        if(_curDailyRank.IsShowEndView)
            return;

        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home || SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome)
        {
            UpdateRobotInfo(true, null);
            
            if (!UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupLevelRankingMain))
            {
                _curDailyRank.IsShowEndView = true;
                UIManager.Instance.OpenUI(UINameConst.UIPopupLevelRankingMain, _curDailyRank);
            }
        }
    }
    
    public bool CheckActivityStart()
    {
        if (!IsOpenActivity())
            return false;

        return !_curDailyRank.IsShowStartView;
    }
    
    public void SendActivityBi(StorageDailyRank dailyRank)
    {
        dailyRank = dailyRank == null ? _curDailyRank : dailyRank;
        dailyRank.Robots.Sort((x, y) =>  y.CurScore - x.CurScore);

        int rank = dailyRank.Robots.Count + 1;
        for (int i = 0; i < dailyRank.Robots.Count; i++)
        {
            if (dailyRank.CurScore >= dailyRank.Robots[i].CurScore)
            {
                rank = i + 1;
                break;
            }
        }
        
        if (rank == 1)
            _dailyRankGroup.WinCount++;
        else if(rank >= 4)
            _dailyRankGroup.LostCount++;
        
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventDailyRankDesc, dailyRank.CurScore.ToString(), rank.ToString());
    }

    public bool IsUserWin()
    {
        foreach (var robot in _curDailyRank.Robots)
        {
            if (robot.CurScore <= _curDailyRank.CurScore)
                continue;

            return false;
        }

        return true;
    }
    
    public void AddScore(int id, Vector3 position)
    {
        if(!IsOpenActivity())
            return;
        
        if(IsActiveTimeEnd(null))
            return;

        if(id == 3000011 || id == 3000012 || id == 3000013 || id == 3000014)
            return;
        
        _curDailyRank.CurScore += 1;

        MergePromptManager.Instance.ShowRankLevelTip(position);
    }

    public bool IsOpenActivity()
    {
        if (_curDailyRank == null)
            return false;
        
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyRank))
            return false;

        return IsOpened();
    }
    
    public bool IsActiveTimeEnd(StorageDailyRank dailyRank)
    {
        dailyRank = dailyRank == null ? _curDailyRank : dailyRank;
        
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyRank))
            return false;
        
        if(dailyRank.ActiveEndTime == 0)
            return false;
        
        return APIManager.Instance.GetServerTime()  > dailyRank.ActiveEndTime;
    }

    public bool IsUpdateTimeEnd()
    {
        return _curDailyRank.UpdateTime == _curDailyRank.ActiveEndTime;
    }
    
    public string GetActiveTime()
    {
        long diffValue;
        if (!IsOpenActivity())
        {
            return "00:00";
        }
        
        diffValue = (long)_curDailyRank.ActiveEndTime - (long)APIManager.Instance.GetServerTime();
        if (diffValue < 0)
            return "00:00";
        return CommonUtils.FormatLongToTimeStr(diffValue);
    }

    private void RootCheat(bool mandatory, StorageDailyRank dailyRank)
    {
        dailyRank = dailyRank == null ? _curDailyRank : dailyRank;
        
        if (!mandatory && dailyRank.ActiveEndTime != dailyRank.UpdateTime)
            return;
            
        dailyRank.Robots.Sort((x,y)=>y.CurScore - x.CurScore);
        
        for (int i = 0; i < dailyRank.Robots.Count; i++)
        {
            if (i > 3)
                break;
            
            if(dailyRank.Robots[i].CurScore > dailyRank.CurScore)
                continue;

            int defaultValue = dailyRank.CurScore;
            if (defaultValue <= 0)
            {
                defaultValue = _dailyRankGroup.DefaultValues[i];

                defaultValue = defaultValue / 3;
            }
            dailyRank.Robots[i].CurScore = defaultValue + (int)(defaultValue * (60 - i * 20) / 100f);
        }
    }
    private int CalculateDifficulty(int diffId, int curValue, int needValue, int changeValue)
    {
        if (needValue == 0 && changeValue == 0)
            return -1;

        if (curValue != needValue)
            return -1;
        
        int tempId = diffId - changeValue;
        TableDrDifficulty difficulty = DailyRankConfigManager.Instance._difficultyData.Find(a => a.id == tempId);
        if (difficulty != null)
            return difficulty.id;

        if (changeValue < 0)
            difficulty = DailyRankConfigManager.Instance._difficultyData[DailyRankConfigManager.Instance._difficultyData.Count - 1];
        else
        {
            difficulty = DailyRankConfigManager.Instance._difficultyData[0];
        }
        
        return difficulty.id;
    }

    private void InitRobots()
    {
        if (_curDailyRank.Robots != null && _curDailyRank.Robots.Count > 0)
            return;
        
        for (int i = 0; i < 4; i++)
        {
            StorageDailyRankRobot robot = new StorageDailyRankRobot();
            _curDailyRank.Robots.Add(robot);
        }
        
        foreach (var robot in _curDailyRank.Robots)
        {
            robot.CurScore = 0;
            robot.RobotName = RandomName();
        }
    }
    
    public string RandomName()
    {
        string name = "";
        for (int i = 0; i < 6; i++)
        {
            name += RandomCodes[Random.Range(0, RandomCodes.Length)];
        }
        return name;  
    }

    public StorageDailyRank CheckActivityEnd()
    {
        List<string> keys = _dailyRankGroup.DailyRanks.Keys.ToList();
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            StorageDailyRank lastActivity = _dailyRankGroup.DailyRanks[keys[i]];
            if(lastActivity.ActiveEndTime == 0)
                continue;
            
            if(lastActivity.IsShowEndView)
                continue;

            if (APIManager.Instance.GetServerTime() > lastActivity.ActiveEndTime)
                return lastActivity;
        }

        return null;
    }
    
    
    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.DailyRank);
    }
    
    public List<ResData> GetRankReward(int rank, StorageDailyRank dailyRank)
    {
        dailyRank = dailyRank == null ? _curDailyRank : dailyRank;
        if (dailyRank == null)
            return null;
        
        if (dailyRank.Rewards.Count < rank)
            return null;

        List<ResData> resDatas = new List<ResData>();
        
        foreach (var storageDailyRankReward in dailyRank.Rewards[rank - 1].Rewards)
        {
            resDatas.Add(new ResData(storageDailyRankReward.Type, storageDailyRankReward.Num));
        }

        return resDatas;
    }
}