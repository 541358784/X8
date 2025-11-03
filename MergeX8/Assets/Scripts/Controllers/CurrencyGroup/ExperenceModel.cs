using System.Collections.Generic;
using Difference;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using Scripts.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public partial class ExperenceModel : Manager<ExperenceModel>
{
    StorageHome storageHome;

    protected override void InitImmediately()
    {
        storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        CorrectLv();
    }

    private void CorrectLv()
    {
        if (storageHome.Level == 0)
            storageHome.Level = 1;
    }

    public void AddExp(int exp)
    {
        storageHome.Exp += exp;
        // MergeHomeBiManager.Instance.SendItemChangeEvent(BiEventMergeHome.Types.Item.Exp.ToString(), exp, (ulong) storageHotel.Experence.Exp, reason);
        // EventDispatcher.Instance.DispatchEvent<bool>(MergeEvent.DO_REFRESH_EXPERENCE, false);
    }

    public int GetExp()
    {
        return storageHome.Exp;
    }

    public float GetPercentExp()
    {
        return storageHome.Exp / (float) GetCurrentLevelTotalExp();
    }

    public int GetCurrentLevelTotalExp()
    {
        int result = -1;
        if (!IsMaxLevel())
            result = GameConfigManager.Instance.LevelList.Find(x => x.lv == storageHome.Level).xp;
        return result;
    }

    public bool IsMaxLevel()
    {
        bool result = false;
        CorrectLv();
        if (storageHome.Level >= GameConfigManager.Instance.LevelList.Count)
            result = true;
        return result;
    }

    public int GetLevel()
    {
        CorrectLv();
        return storageHome.Level;
    }

    public bool IsCanLevelUp()
    {
        bool result = false;
        if (!IsMaxLevel())
            result = storageHome.Exp >= GetCurrentLevelTotalExp();
        return result;
    }

    public void LevelUp()
    {
        if (!IsCanLevelUp())
            return;
        UserData.Instance.ConsumeRes(UserData.ResourceId.Exp, GetCurrentLevelTotalExp(),
            new GameBIManager.ItemChangeReasonArgs
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.LevelUp,
            });
        
        var config = ExperenceModel.Instance.GetCurrentLevelConfig();
        if (config != null)
        {
            if (DifferenceManager.Instance.IsDiffPlan_New())
            {
                AddRewards(config.planb_reward == null ? config.reward : config.planb_reward, config.amount);
            }
            else
            {
                AddRewards(config.reward, config.amount);
            }
        }
        
        // storageHome.Exp -= GetCurrentLevelTotalExp();
        storageHome.Level += 1;
        TeamManager.Instance.UploadMyInfo();  
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.OnLevelUp, storageHome.Level);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventLevel,data1:storageHome.Level.ToString());
        // MergeHomeBiManager.Instance.SendItemChangeEvent(BiEventMergeHome.Types.Item.Exp.ToString(), -GetCurrentLevelTotalExp(), (uint) storageHotel.Experence.Exp,
        //     new MergeHomeBiManager.ItemChangeReasonArgs {reason = BiEventMergeHome.Types.ItemChangeReason.LevelUp});
        EventDispatcher.Instance.DispatchEvent(MergeEvent.DO_REFRESH_EXPERENCE, true);
        GameBIManager.Instance.SendReachLevelThirdBI(storageHome.Level);
        MainOrderManager.Instance.TryFillOrder();
    }
    
    private void AddRewards(int[] rewards, int[] count)
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            if (UserData.Instance.IsResource(rewards[i]))
            {
                UserData.Instance.AddRes(rewards[i], count[i], new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.LevelUp,
                });
            }
            else
            {
                var mergeItem = MergeManager.Instance.GetEmptyItem();
                mergeItem.Id = rewards[i];
                mergeItem.State = 1;
                MergeManager.Instance.AddRewardItem(mergeItem, MergeBoardEnum.Main,count[i]);
                SendLevelUpBi(rewards[i], count[i], ExperenceModel.Instance.GetLevel());
            }
        }
    }
    
    private void SendLevelUpBi(int id, int count, int level)
    {
        for (int i = 0; i < count; i++)
        {
            var config = GameConfigManager.Instance.GetItemConfig(id);
            if (config == null)
                return;
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventCooking.Types.MergeEventType.MergeItemChangeLevelUp,
                itemAId = config.id,
                ItemALevel = config.level,
                isChange = true,
                extras = new Dictionary<string, string>
                {
                    {"level", level.ToString()}
                }
            });
        }
    }
    

    public TableLevel GetCurrentLevelConfig()
    {
        var config = GameConfigManager.Instance.LevelList.Find(x => x.lv == storageHome.Level);
        return config;
    } 
    public TableLevel GetCurrentLevelConfigByLevel(int level)
    {
        var config = GameConfigManager.Instance.LevelList.Find(x => x.lv == level);
        return config;
    }
}