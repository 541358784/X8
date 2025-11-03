using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.GarageCleanup;
using DragonPlus.Config.PayRebate;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class GarageCleanupModel : ActivityEntityBase
{
    private static GarageCleanupModel _instance;
    public static GarageCleanupModel Instance => _instance ?? (_instance = new GarageCleanupModel());
    private int[] leftD = {0,6,12,18,24 };
    private int[] rightD = {4,8,12,16,20 };

    public StorageGarageCleanup _storageGarageCleanup;

    public StorageGarageCleanup StorageGarageCleanup
    {
        get
        {
            if (_storageGarageCleanup == null)
            {
                var garageCleanup = StorageManager.Instance.GetStorage<StorageHome>().GarageCleanup;
                if (!garageCleanup.ContainsKey(StorageKey))
                {
                    garageCleanup.Add(StorageKey, new StorageGarageCleanup());
                    var level = ExperenceModel.Instance.GetLevel();
                    var levelConfigs = GarageCleanupConfigManager.Instance.GetConfig<GarageCleanupLevelGroupConfig>();
                    foreach (var config in levelConfigs)
                    {
                        if (config.LevelRange[0] <= level && config.LevelRange[1] >= level)
                        {
                            garageCleanup[StorageKey].LevelGroup = config.Id;
                        }
                    }
                }
                _storageGarageCleanup = garageCleanup[StorageKey];
            }
           
            return _storageGarageCleanup;
        }
    }

    public override string Guid => "OPS_EVENT_TYPE_GARAGE_CLEANUP";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        GarageCleanupConfigManager.Instance.InitConfig(configJson);
        InitServerDataFinish();
        DebugUtil.Log($"InitConfig:{Guid}");
    }

    public override void UpdateActivityState()
    {
        //InitServerDataFinish();
    }
    
    protected override void InitServerDataFinish()
    {
        _storageGarageCleanup = null;
        StorageGarageCleanup.EndTime = (long)EndTime;
        MatchCleanUp();
    }

    public bool IsOpened()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GarageCleanup))
            return false;

        bool isOpen = base.IsOpened();
        if (!isOpen)
            return false;
        if (IsActivityFinish())
            return false;
        return true;
    }

    public void StartActivity()
    {
        _storageGarageCleanup.IsShowStart = true;
        GenData();
    }

    public int GetProgress()
    {
        var board = GetBoard();
        int finishCount = 0;
        foreach (var item in board.Items)
        {
            if (item.State == 1)
                finishCount++;
        }

        return finishCount * 4;
    }
    public bool IsFinishLevel()
    {
        var board = GetBoard();
        foreach (var item in board.Items)
        {
            if (item.State != 1)
                return false;
        }

        return true;
    }

    public void UnlockLevel()
    {
        GetBoard().IsStart = true;
    }
    /// <summary>
    /// 生成棋盘
    /// </summary>
    public void GenData()
    {
        if(!IsOpened())
            return;
        if (StorageGarageCleanup.Boards.Count > 0)
            return;
        var configs= GetGarageCleanupConfig();
       for (int i = 0; i < configs.Count; i++)
       {
           StorageGarageCleanupBoard board = new StorageGarageCleanupBoard();
           var itemConfigs=GetGarageCleanupBoard(i );
           if (itemConfigs != null && itemConfigs.Count > 0)
           {
               for (int j = 0; j < itemConfigs.Count; j++)
               {
                   StorageGarageCleanupBoardItem item = new StorageGarageCleanupBoardItem();
                   item.Id = itemConfigs[j].ItemId;
                   
                   board.Items.Add(item);
               }    
           }

           if (configs[i].UnlockConsume == null || configs[i].UnlockConsume.Count < 2)
               board.IsStart = true;
               
           StorageGarageCleanup.Boards.Add(board);
       }

       MatchCleanUp();
    }

    private void MatchCleanUp()
    {
        if(StorageGarageCleanup.Boards.Count ==  0)
            return;

        foreach (var board in StorageGarageCleanup.Boards)
        {
            foreach (var item in board.Items)
            {
                var recovery = GameConfigManager.Instance.GetRecovery(item.Id);
                if(recovery == null)
                    continue;

                item.Id = recovery.task_replace;
            }
        }
    }
    
    public void TurnIn(int index,bool isUseToken)
    {
        GetBoard().Items[index].State = 1;
        var config = GetGarageCleanupBoard(StorageGarageCleanup.Level, index);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFishbondCleanSubmit,(StorageGarageCleanup.Level+1).ToString(),config.ItemId.ToString(),StorageGarageCleanup.LevelGroup.ToString());

        var rewards = GarageCleanupModel.Instance.GetTurnInReward(index);
        var lines=GetLine(index);
        EventDispatcher.Instance.DispatchEvent(EventEnum.GARAGE_CLEANUP_TURNIN,MergeBoardEnum.Main,index,rewards,lines);
        if (isUseToken)
        {
            UserData.Instance.ConsumeRes(UserData.ResourceId.Fishpond_token,GetFishTokenCount(index),new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug
            });
        }
        else
        {
            Dictionary<int, int> demands = new Dictionary<int, int>();
            demands.Add(GetBoard().Items[index].Id,1);
            if (demands != null)
            {
                foreach (var kv in demands)
                {
                    var product = GameConfigManager.Instance.GetItemConfig(kv.Key);
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventCooking.Types.MergeEventType.MergeChangeReasonGarageCleanupConsume,
                        itemAId = product.id,
                        ItemALevel = product.level,
                        isChange = true,
                    });
                }
            }
            MergeManager.Instance.Consume(demands,MergeBoardEnum.Main);

        }
    
        foreach (var res in rewards)
        {
            if (UserData.Instance.IsResource(res.id))
            {
                UserData.Instance.AddRes(res.id, res.count,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventCooking.Types.ItemChangeReason.GarageCleanGet
                    }, false);
            }
            else
            {
                for (int i = 0; i < res.count; i++)
                {
                    var mergeItem = MergeManager.Instance.GetEmptyItem();
                    mergeItem.Id = res.id;
                    mergeItem.State = 1;
                    MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonGarageCleanupGet,
                        isChange = true,
                    });
                }
               
              
            }
        }
        if (IsFinishLevel())
        {
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFishbondCleanReward,(StorageGarageCleanup.Level+1).ToString());

            StorageGarageCleanup.Level++;
            EventDispatcher.Instance.DispatchEvent(EventEnum.GARAGE_CLEANUP_LevelFinish,MergeBoardEnum.Main);
            if (StorageGarageCleanup.Level > StorageGarageCleanup.Boards.Count - 1)
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.GARAGE_CLEANUP_Finish);
            }
          
        }
    }
    

    public bool IsActivityFinish()
    {
        if (StorageGarageCleanup.Level!=0 && StorageGarageCleanup.Level > StorageGarageCleanup.Boards.Count - 1)
        {
            return true;
        }
        return false;
    }
    
    public bool IsActivityFinish(StorageGarageCleanup storageData)
    {
        if (storageData.Level!=0 && storageData.Level > storageData.Boards.Count - 1)
        {
            return true;
        }
        return false;
    }
    
    public bool IsEndSoon()
    {
        if (StorageGarageCleanup.Level >= StorageGarageCleanup.Boards.Count - 1)
        {
            return true;
        }
        return false;
    }
    
    public StorageGarageCleanupBoardItem GetBoardItem(int index)
    {
        return GetBoard().Items[index];
    }

    public int GetRevealNum(StorageGarageCleanup storage)
    {
        int num = 0;
        foreach (var storageGarageCleanupBoard in storage.Boards)
        {
            foreach (var storageGarageCleanupBoardItem in storageGarageCleanupBoard.Items)
            {
                if (storageGarageCleanupBoardItem.State == 1)
                    num++;
            }
        }

        return num;
    }
    public void RevealBoard()
    {
        GetBoard().IsReveal = true;
    }
    public List<ResData> GetTurnInReward(int index)
    {
        List<ResData> resDatas = new List<ResData>();
        var config = GetGarageCleanupBoard(StorageGarageCleanup.Level, index);
        for (int i = 0; i < config.UnlockReward.Count; i++)
        {
            ResData resData = new ResData(config.UnlockReward[i], config.UnlockRewardCount[i]);
            resDatas.Add(resData);
        }
        var res = CheckExtendReward(index);
        if(res!=null&& res.Count>0)
            resDatas.AddRange(res);
        return resDatas;
    }
    
    public int  GetFishTokenCount(int index)
    {
        var config = GetGarageCleanupBoard(StorageGarageCleanup.Level, index);
        if(config==null)
            return 0;
        return config.Fishpondtoken;
    }
    
    
    /// <summary>
    /// 检查连线奖励
    /// </summary>
    public List<ResData> CheckExtendReward(int index )
    {
        List<ResData> resDatas = new List<ResData>();
        var board = GetBoard();
        var config = GetGarageCleanupConfigByLevel();
        int x = index % 5;
        int y = index / 5;
        bool flag = true;
        //竖排
        for (int i = 0; i < 5; i++)
        {
            int _index = x  + i* 5;
            if (board.Items[_index].State != 1 && _index != index)
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            for (int i = 0; i < config.RowReward.Count; i++)
            {
                ResData resData = new ResData(config.RowReward[i], config.RowRewardCount[i]);
                resDatas.Add(resData);
            }
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFishbondCleanBinggo,(StorageGarageCleanup.Level+1).ToString(),"1");

        }

        //横排
        flag = true;
        for (int i = 0; i < 5; i++)
        {
            int _index = y * 5 + i;
            if (board.Items[_index].State != 1 && _index != index)
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            for (int i = 0; i < config.RowReward.Count; i++)
            {
                ResData resData = new ResData(config.RowReward[i], config.RowRewardCount[i]);
                resDatas.Add(resData);
            }
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFishbondCleanBinggo,(StorageGarageCleanup.Level+1).ToString(),"2");

        }
        
        //斜线
        flag = true;
        if (leftD.Contains(index))
        {
            foreach (var i in leftD)
            {
                if (board.Items[i].State != 1 && i!=index)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                for (int i = 0; i < config.DiagonalsReward.Count; i++)
                {
                    ResData resData = new ResData(config.DiagonalsReward[i], config.DiagonalsRewardCount[i]);
                    resDatas.Add(resData);
                }
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFishbondCleanBinggo,(StorageGarageCleanup.Level+1).ToString(),"3");

            }

        }  
       
        flag = true;
        if (rightD.Contains(index))
        {
            foreach (var i in rightD)
            {
                if (board.Items[i].State != 1 && i!=index)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                for (int i = 0; i < config.DiagonalsReward.Count; i++)
                {
                    ResData resData = new ResData(config.DiagonalsReward[i], config.DiagonalsRewardCount[i]);
                    resDatas.Add(resData);
                }
            }
        }
        flag = true;
        for (int i = 0; i < board.Items.Count; i++)
        {
            if (board.Items[i].State != 1 && i != index)
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            for (int i = 0; i < config.FullReward.Count; i++)
            {
                ResData resData = new ResData(config.FullReward[i], config.FullRewardCount[i]);
                resDatas.Add(resData);
            }
        }
        
        return resDatas;
    }

    public List<List<int>> GetLine(int index)
    {
        List<List<int>> lineList = new List<List<int>>();
        var board = GetBoard();
        int x = index % 5;
        int y = index / 5;
        bool flag = true;
        List<int> line = new List<int>();
        for (int i = 0; i < board.Items.Count; i++)
        {
            line.Add(i);
            if (board.Items[i].State != 1 && i != index)
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            lineList.Add(line);
            return lineList;
        }
 
        //竖排
        flag = true;
        line = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int _index = x  + i* 5;
            line.Add(_index);
            if (board.Items[_index].State != 1 && _index != index)
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            lineList.Add(line);
        }
        //横排
        flag = true;
        line = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int _index = y * 5 + i;
            line.Add(_index);
            if (board.Items[_index].State != 1 && _index != index)
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            lineList.Add(line);
        }
        
        //斜线
        flag = true;
        line = new List<int>();
        if (leftD.Contains(index))
        {
            foreach (var i in leftD)
            {
                line.Add(i);
                if (board.Items[i].State != 1 && i!=index)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                lineList.Add(line);
            }

        }  
       
        flag = true;
        line = new List<int>();
        if (rightD.Contains(index))
        {
            foreach (var i in rightD)
            {
                line.Add(i);
                if (board.Items[i].State != 1 && i!=index)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                lineList.Add(line);
            }
        }
      
        return lineList;
    }

    public bool IsCanGet()
    {
        if (!IsOpened())
            return false;
        if (!StorageGarageCleanup.IsShowStart)
            return false;
        Dictionary<int, int> mergeItemCounts = MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.Main);
     
        foreach (var item in GetBoard().Items)
        {
            if (mergeItemCounts.ContainsKey(item.Id)&& item.State!=1)
                return true;
        }
        return false;
    }
    public bool IsHave(int itemId)
    { 
        if (ConfigurationController.Instance.version == VersionStatus.DEBUG && SROptions.Current.IsHaveAll)
            return true;
        Dictionary<int, int> mergeItemCounts = MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.Main);

        if (mergeItemCounts.ContainsKey(itemId))
            return true;

        return false;
    }

    public bool IsReveal()
    {
        if (!IsOpened())
            return false;

        return GetBoard().IsReveal;
    }
    
    public bool IsTaskNeedItem(int itemId)
    {
        if (!IsOpened())
            return false;
        foreach (var item in GetBoard().Items)
        {
            if (item.Id == itemId && item.State!=1)
                return true;
        }

        return false;
    }
    
    /// <summary>
    /// 获得当前棋盘
    /// </summary>
    /// <returns></returns>
    public StorageGarageCleanupBoard GetBoard()
    {
        if (StorageGarageCleanup.Level == 0 && StorageGarageCleanup.Boards.Count <= 0)
        {
            GenData();
        }
        if (StorageGarageCleanup.Level > StorageGarageCleanup.Boards.Count - 1)
            return StorageGarageCleanup.Boards[StorageGarageCleanup.Boards.Count - 1];
        return StorageGarageCleanup.Boards[StorageGarageCleanup.Level];
        
    }

    public StorageGarageCleanupBoard GetBoard(int level)
    {
        return StorageGarageCleanup.Boards[level];
    }

    public int GetCleanupLevel()
    {
        return StorageGarageCleanup.Level;
    }

    private Dictionary<int, List<GarageCleanupConfig>> LevelGroupConfigDic = new Dictionary<int, List<GarageCleanupConfig>>();
    public List<GarageCleanupConfig> GetGarageCleanupConfig()
    {
        if (!LevelGroupConfigDic.ContainsKey(StorageGarageCleanup.LevelGroup))
        {
            var allConfigs = GarageCleanupConfigManager.Instance.GetConfig<GarageCleanupConfig>();
            var configs = new List<GarageCleanupConfig>();
            foreach (var config in allConfigs)
            {
                if (config.LevelGroup == StorageGarageCleanup.LevelGroup)
                    configs.Add(config);
            }
            LevelGroupConfigDic.Add(StorageGarageCleanup.LevelGroup,configs);   
        }
        return LevelGroupConfigDic[StorageGarageCleanup.LevelGroup];
    }
    
    public GarageCleanupConfig GetGarageCleanupConfigByLevel()
    {
        return GetGarageCleanupConfig().Find(a=>a.Level==StorageGarageCleanup.Level);
    }    
    private Dictionary<int, List<GarageCleanupBoard>> LevelGroupBoardConfigDic = new Dictionary<int, List<GarageCleanupBoard>>();
    public List<GarageCleanupBoard> GetGarageCleanupBoard()
    {
        if (!LevelGroupBoardConfigDic.ContainsKey(StorageGarageCleanup.LevelGroup))
        {
            var allConfigs = GarageCleanupConfigManager.Instance.GetConfig<GarageCleanupBoard>();
            var configs = new List<GarageCleanupBoard>();
            foreach (var config in allConfigs)
            {
                if (config.LevelGroup == StorageGarageCleanup.LevelGroup)
                    configs.Add(config);
            }
            LevelGroupBoardConfigDic.Add(StorageGarageCleanup.LevelGroup,configs);   
        }
        return LevelGroupBoardConfigDic[StorageGarageCleanup.LevelGroup];
    }
    
    public List<GarageCleanupBoard> GetGarageCleanupBoard(int level)
    {
        return GetGarageCleanupBoard().FindAll(a=>a.Level==level);
    }
   public GarageCleanupBoard GetGarageCleanupBoard(int level,int index)
   {
        return GetGarageCleanupBoard(level).Find(a=>a.Index==index);
   }

   public bool CanShowUI()
   {
       if (CheckActivityEnd() != null)
           return false;

       if (!IsOpened())
           return false;

       if (StorageGarageCleanup.IsShowStart)
           return false;
       return true;
   }   
   
   public bool CanShowPackInStore()
   {
       if (!IsOpened())
           return false;
       return true;
       return StorageGarageCleanup.PackBuyCount<3;
   }

   public void BuyCleanupPack()
   {
       StorageGarageCleanup.PackBuyCount++;
       EventDispatcher.Instance.DispatchEvent(EventEnum.GARAGE_CLEANUP_PURCHASE_REFRESH);
   }

   public int GetCleanupPackLeft()
   {
       return 3-StorageGarageCleanup.PackBuyCount;
   }
   
   public StorageGarageCleanup CheckActivityEnd()
   {
       var garageCleanup = StorageManager.Instance.GetStorage<StorageHome>().GarageCleanup;
       List<string> keys = garageCleanup.Keys.ToList();
       for (int i = keys.Count - 1; i >= 0; i--)
       {
           StorageGarageCleanup lastActivity = garageCleanup[keys[i]];
           
           if(!lastActivity.IsShowStart)
               continue;

           if(lastActivity.EndTime == 0)
               continue;


           if ((long)APIManager.Instance.GetServerTime() > lastActivity.EndTime || lastActivity.IsActivityEnd)
               garageCleanup.Remove(keys[i]);
       }

       return null;
   }

   public void UpdateActivity()
   {
       StorageGarageCleanup cleanupData = GarageCleanupModel.Instance.CheckActivityEnd();
       if (cleanupData == null)
           return;
  
       UIGarageCleanupMainController mainController = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIGarageCleanupMain) as UIGarageCleanupMainController;
       if(mainController != null)
           return;
        
       UIPopupGarageCleanupSubmitController controller = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupGarageCleanupSubmit) as UIPopupGarageCleanupSubmitController;
       if (controller != null)
       {  
           controller?.AnimCloseWindow(() =>
           {
               cleanupData.IsActivityEnd = true;
               // UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupEnd, cleanupData);
           });
       }
       else
       {
           cleanupData.IsActivityEnd = true;
          // var end= UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupEnd, cleanupData);
          // if (end == null)
          //     cleanupData.IsActivityEnd = true;
       }
   }
   public override bool CanDownLoadRes()
   {
       return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.GarageCleanup);
   }
}