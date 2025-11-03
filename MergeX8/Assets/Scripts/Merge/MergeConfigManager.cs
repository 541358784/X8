using System;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine.U2D;

public class MergeConfigManager : Manager<MergeConfigManager>
{
    class ConfigTemplate
    {
        public List<TableMergeItem> ItemConfig;
        public List<TableBoardGrid> BoardGrid;
    }

    private List<TableMergeItem> tempProductParent = new List<TableMergeItem>();
    private SpriteAtlas m_mergeicon;
    private Dictionary<int, int> maxLevelIds = new Dictionary<int, int>();

    public SpriteAtlas mergeIcon
    {
        get
        {
            if (m_mergeicon == null)
            {
                SpriteAtlas atlas = ResourcesManager.Instance.LoadSpriteAtlasVariant(AtlasName.MergeIconAtlas);
                m_mergeicon = atlas;
                return m_mergeicon;
            }
            else
            {
                return m_mergeicon;
            }
        }
    }

    protected override void InitImmediately()
    {
        base.InitImmediately();
    }

    public void InitConfigs()
    {
    }

    public TableMergeItem GetLastLevelItemConfig(int id) // 获取上一级的物品
    {
        TableMergeItem mergeItemConfig = GameConfigManager.Instance.MergeItemList.Find(x => x.next_level == id);
        return mergeItemConfig;
    }

    public bool IsMaxEnergy(TableMergeItem item)
    {
        bool result = false;
        if (item != null)
            if (item.type ==  (int)MergeItemType.energy && item.next_level == -1)
            {
                result = true;
            }

        return result;
    }

    public int GetItemLevel(int id)
    {
        int result = -1;
        TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (mergeItemConfig != null)
            result = mergeItemConfig.level;
        return result;
    }

    public int GetOneOutput(TableMergeItem mergeItemConfig)
    {
        int output = -1;
        if (mergeItemConfig.output != null)
        {
            int power = mergeItemConfig.power.GetRandomWithPower();
            output = mergeItemConfig.output[power];
        }

        if (output == 0)
            output = -1;
        return output;
    }
    public int GetOneOutputByLimit(TableMergeItem mergeItemConfig)
    {
        int output = -1;
        if (mergeItemConfig.output != null)
        {
            var level=ExperenceModel.Instance.GetLevel();
            List<int> outputTemp = new List<int>();
            List<int> outputPowerTemp = new List<int>();
            for (int i = 0; i < mergeItemConfig.output.Length; i++)
            {
                if (level >= mergeItemConfig.outputLimit[i])
                {
                    outputTemp.Add(mergeItemConfig.output[i]);
                    outputPowerTemp.Add(mergeItemConfig.power[i]);
                }
            
            }
            var index = CommonUtils.RandomIndexByWeight(outputPowerTemp);
            output = outputTemp[index];
        }

        if (output == 0)
            output = -1;
        return output;
    }    
    
    public int GetOneOutputByLimitDynamicPower(TableMergeItem mergeItemConfig,int index,MergeBoardEnum boardId)
    {
        int output = -1;
        var storageItem =MergeManager.Instance.GetBoardItem(index,boardId);
        if (storageItem == null)
            return output;

        if (mergeItemConfig.output != null)
        {
            List<int> outputTemp = new List<int>();
            List<int> outputPowerTemp = new List<int>();
            for (int i = 0; i < mergeItemConfig.output.Length; i++)
            {
                int noProductCount = 0;
                if(storageItem.DropIntervalDic.ContainsKey(mergeItemConfig.output[i]))
                    noProductCount=storageItem.DropIntervalDic[mergeItemConfig.output[i]];
                if (mergeItemConfig.outputLimitByTimes==null||noProductCount >= mergeItemConfig.outputLimitByTimes[i])
                {
                    outputTemp.Add(mergeItemConfig.output[i]);
                    outputPowerTemp.Add(mergeItemConfig.power[i]);
                }
            }
            
            //根据任务调整物品产出权重
            if (mergeItemConfig.dynamicPowerItem != null && mergeItemConfig.dynamicPowerItem.Length > 0)
            {
                int taskNeedCount=0;
                int taskNeedId=0;
                for (int i = 0; i < mergeItemConfig.dynamicPowerItem.Length; i++)
                {
                    var mergeLine= GetMergeLine(mergeItemConfig.dynamicPowerItem[i]);
                    if (mergeLine != null)
                    {
                        for (int j = 0; j < mergeLine.output.Length; j++)
                        {
                            if (MainOrderManager.Instance.IsTaskNeedItem(mergeLine.output[j]))
                            {
                                taskNeedCount++;
                                taskNeedId = mergeItemConfig.dynamicPowerItem[i];
                                break;
                            }
                        }
                    }
                }

                if (taskNeedCount == 1)
                {
                    for (int i = 0; i < mergeItemConfig.dynamicPowerItem.Length; i++)
                    {
                        if (mergeItemConfig.dynamicPowerItem[i] == taskNeedId)
                        {
                            if(outputTemp.Contains(mergeItemConfig.dynamicPowerItem[i]))
                            {
                                int idx = outputTemp.IndexOf(mergeItemConfig.dynamicPowerItem[i]);
                                int power= outputPowerTemp[idx];
                                power =power - power * mergeItemConfig.dynamicPower / 100;
                                outputPowerTemp[idx]=power;
                            }
                        }
                        else
                        {
                            if(outputTemp.Contains(mergeItemConfig.dynamicPowerItem[i]))
                            {
                                int idx = outputTemp.IndexOf(mergeItemConfig.dynamicPowerItem[i]);
                                int power= outputPowerTemp[idx];
                                power = power * (100+mergeItemConfig.dynamicPower) / 100;
                                outputPowerTemp[idx]=power;
                            }
                        }
                    }
                }
            }
            
            var p = CommonUtils.RandomIndexByWeight(outputPowerTemp);
            output = outputTemp[p];
        }

        if (output == 0)
            output = -1;

        return output;
    }
    public int GetOneTimeOutput(TableMergeItem mergeItemConfig)
    {
        int output = -1;
        if (mergeItemConfig.timeOutput != null)
        {
            int power = mergeItemConfig.timeOutputPower.GetRandomWithPower();
            output = mergeItemConfig.timeOutput[power];
        }

        if (output == 0)
            output = -1;
        return output;
    }

    public bool IsCanShowStar(TableMergeItem mergeItemConfig)
    {
        if (mergeItemConfig.next_level > 0)
            return false;
       
        return true;
    }
    public bool IsCanProductItem(TableMergeItem mergeItemConfig)
    {
        if (mergeItemConfig == null)
            return false;

        bool result = false;
        if (mergeItemConfig != null)
        {
            result = mergeItemConfig.output != null && !mergeItemConfig.output.Contains(0) &&
                     MergeManager.Instance.GetOutputAmount(mergeItemConfig) > 0;
        }

        return result;
    }
    public bool IsEnergyTorrentProduct(TableMergeItem mergeItemConfig)
    {
        if (mergeItemConfig == null)
            return false;

        return mergeItemConfig.energy_frenzy;
    }

    public bool IsCanProductItem(int id)
    {
        bool result = false;
        var mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (mergeItemConfig == null)
            return result;
        result = mergeItemConfig.output != null && !mergeItemConfig.output.Contains(0) &&
                 MergeManager.Instance.GetOutputAmount(mergeItemConfig) > 0;
        return result;
    }

    public bool IsEnergyProductItem(TableMergeItem mergeItemConfig) //是否是体力产出
    {
        bool result = false;
        if (mergeItemConfig == null)
        {
            return result;
        }
        result =mergeItemConfig.output_cost!=null &&  IsCanProductItem(mergeItemConfig) && mergeItemConfig.output_cost.Length >= 2 &&
                 mergeItemConfig.output_cost[0] == 1; // 1体力 2时间
        return result;
    }

    public bool IsProductItem(TableMergeItem mergeItemConfig) //是否是体力产出
    {
        bool result = false;
        result = IsCanProductItem(mergeItemConfig) ;
        return result;
    }

    public bool IsEnergyProductItem(int id) //是否是体力产出
    {
        bool result = false;
        var mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (mergeItemConfig == null)
            return result;
        result = IsCanProductItem(mergeItemConfig) && mergeItemConfig.output_cost.Length >= 2 &&
                 mergeItemConfig.output_cost[0] == 1; // 1体力 2时间
        return result;
    }

    public bool IsTimeProductItem(TableMergeItem mergeItemConfig) //是否是时间产出
    {
        bool result = false;
        if (mergeItemConfig == null || mergeItemConfig.timeOutput_cost == null)
            return result;
        result = mergeItemConfig.timeOutput_cost.Length >= 2 && mergeItemConfig.timeOutput_cost[0] == 2; // 1体力 2时间
        return result;
    }

    public bool IsTimeProductItem(int id) //是否是时间产出
    {
        TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
        return IsTimeProductItem(mergeItemConfig);
    }

    public bool IsDeathProductItem(TableMergeItem mergeItemConfig, StorageMergeItem storageMergeItem,
        bool checkOut = true) //是否是死亡产出
    {
        if (mergeItemConfig == null || storageMergeItem == null)
            return false;

        if (checkOut)
        {
            if (mergeItemConfig?.out_death == 0)
                return false;
        }

        if (mergeItemConfig?.onelife <= 0)
            return false;

        return storageMergeItem.ProductWheel >= mergeItemConfig?.onelife;
    }  
    
    public bool IsLimitNoCdProductItem(TableMergeItem mergeItemConfig) //无CD限制建筑
    {
        if (mergeItemConfig == null )
            return false;

        if (mergeItemConfig.ignoreCdProp)
            return true;
        
        return  mergeItemConfig?.onelife>0  ||mergeItemConfig?.out_death>0 || mergeItemConfig.type==(int) MergeItemType.eatBuild;
    }
    public bool IsDeathBoxItem(TableMergeItem mergeItemConfig, StorageMergeItem storageMergeItem) //是否是宝箱类型
    {
        if (mergeItemConfig == null || storageMergeItem == null)
            return false;

        if (mergeItemConfig?.type !=  (int)MergeItemType.box)
            return false;

        if (mergeItemConfig?.onelife <= 0)
            return false;

        return storageMergeItem.ProductWheel >= mergeItemConfig?.onelife;
    }

    public void InitMaxLevelId()
    {
        if (GameConfigManager.Instance == null)
            return;
        if (GameConfigManager.Instance.MergeItemList == null)
            return;
        var max = GameConfigManager.Instance.MergeItemList.FindAll(x => x.next_level == -1);
        if (max == null)
            return;

        maxLevelIds.Clear();
        for (int i = 0; i < max.Count; i++)
        {
            int id = max[i].id;
            if (maxLevelIds.ContainsKey(id))
                continue;

            maxLevelIds.Add(id, id);
        }
    }

    public bool IsMaxLevel(int id)
    {
        if (maxLevelIds == null)
            return false;

        if (maxLevelIds.ContainsKey(id))
            return true;

        return false;
    }

    public bool IsCanMergeItem(int id)
    {
        bool result = false;
        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(id);
        result = config?.next_level != -1 || config.type== (int)MergeItemType.universal || config.type== (int)MergeItemType.split || config.type== (int)MergeItemType.MagicWand ;
        return result;
    }


    public bool IsDeathBoxItem(int id, StorageMergeItem storageMergeItem) //是否是宝箱类型
    {
        return IsDeathBoxItem(GameConfigManager.Instance.GetItemConfig(id), storageMergeItem);
    }

    public ActiveCostType GetActiveCostType(int id)
    {
        ActiveCostType type = ActiveCostType.none;
        var mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (mergeItemConfig == null)
            return type;

        if (mergeItemConfig.active_cost == null)
            return type;

        if (mergeItemConfig.active_cost.Length <= 1)
            return type;
        type = (ActiveCostType) mergeItemConfig.active_cost[0];
        return type;
    }

    public bool IsStoreItem(int id) //是否是存贮产出量建筑
    {
        bool result = false;
        var mergeItemConfig = GameConfigManager.Instance.GetItemConfig(id);
        result = MergeManager.Instance.GetMaxOutputAmount(mergeItemConfig) > 0;
        return result;
    }

    public bool IsStoreItem(TableMergeItem mergeItemConfig) //是否是存贮产出量建筑
    {
        return MergeManager.Instance.GetMaxOutputAmount(mergeItemConfig) > 0;
    }

    public bool IsTimeStoreItem(TableMergeItem mergeItemConfig) //是否是存贮时间产出量建筑
    {
        return mergeItemConfig?.time_max_output_amount > 0;
    }

    public List<TableMergeItem> GetProductParent(int id) //获取来源
    {
        tempProductParent.Clear();
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return tempProductParent;
        int in_line = config.in_line;
        var line = GameConfigManager.Instance.GetMergeLine(in_line);
        Array.Sort(line.output, (x, y) => x - y);
        if (line.output.Length == 0)
            return tempProductParent;
        int smallId = line.output[0];
        tempProductParent =
            GameConfigManager.Instance.MergeItemList.FindAll(x => x.output.Contains(smallId) || x.out_death == smallId);
        return tempProductParent;
    }

    public int GetMergeLineById(int id) //获取合成链
    {
        int result = 0;
        var line = GameConfigManager.Instance.MergeLineList.Find(x => x.output.Contains(id));
        if (line != null)
            result = line.id;
        return result;
    }

    public TableMergeLine GetMergeLine(int id) //获取合成链
    {
        var line = GameConfigManager.Instance.MergeLineList.Find(x => x.output.Contains(id));
        if (line != null)
            return line;
        return null;
    }

    public TableMergeLine GetMergeLineByLine(int line) //获取合成链
    {
        var mergeLine = GameConfigManager.Instance.GetMergeLine(line);
        if (mergeLine != null)
            return mergeLine;
        return null;
    }

    public int GetMergeLineProductItemId(int id, int productCount) //获取合成链产出次数产出物id
    {
        int result = 0;
        int line = GetMergeLineById(id);
        if (line == 0)
            return result;
        var mergeLine = GameConfigManager.Instance.GetMergeLine(line);
        if (mergeLine == null)
            return result;
        if (mergeLine.presetDropQueue == null)
            return result;
        if (productCount >= mergeLine.presetDropQueue.Length)
        {
            if (mergeLine.presetDropQueue1 == null)
                return result;
            if (productCount >= (mergeLine.presetDropQueue.Length + mergeLine.presetDropQueue1.Length))
                return result;

            return mergeLine.presetDropQueue1[productCount - mergeLine.presetDropQueue.Length];
        }

        result = mergeLine.presetDropQueue[productCount];
        return result;
    }

    public MergeItemType GetItemType(int id)
    {
        MergeItemType type = MergeItemType.none;
        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(id);
        if (config != null)
        {
            type = (MergeItemType) config.type;
        }

        return type;
    }

    public bool IsMagicWand(int id) // 是否是魔杖道具
    {
        bool result = false;
        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(id);
        result = config?.type == (int)MergeItemType.MagicWand;
        return result;
    }
    public bool IsOmnipoten(int id) // 是否是万能道具
    {
        bool result = false;
        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(id);
        result = config?.type == (int)MergeItemType.universal;
        return result;
    }
    public bool IsSplit(int id) // 是否是分解道具
    {
        bool result = false;
        TableMergeItem config = GameConfigManager.Instance.GetItemConfig(id);
        result = config?.type ==  (int)MergeItemType.split;
        return result;
    }
    public TableMergeItem GetXpItem(int level = 1)
    {
        return GameConfigManager.Instance.MergeItemList.Find(x => x.type ==  (int)MergeItemType.exp && x.level == level);
    }

    public bool IsOpenRvSpeedUp(int id)
    {
        bool reslut = false;
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return reslut;
        int line = config.in_line;
        reslut = config.rv_speedup;
        // if (reslut)
        //     reslut = UnlockManager.IsOpen(UnlockManager.MergeUnlockType.RVUnlockLevel);
        return reslut;
    }

    public bool IsCanOmnipoten(TableMergeItem item) // 是否能用王能合成道具 参与合成
    {
        bool reslut = false;
        if (item == null)
            return reslut;
        reslut = item.type == (int) MergeItemType.item || item.type == (int) MergeItemType.diamonds  || item.type ==  (int) MergeItemType.energy || item.type == (int) MergeItemType.decoCoin || item.type == (int) MergeItemType.exp ||
                 item.type == (int) MergeItemType.energy;
        return reslut;
    }
    public bool IsCanSplit(int id) // 能否分解  类型为1 并且等级大于2 有1空格子
    {
        bool result = false;
        var itemConfig = GameConfigManager.Instance.GetItemConfig(id);
        if (itemConfig == null)
            return result;
        if (itemConfig.level < 2 )
            return result;
        
        result = itemConfig.type == (int)MergeItemType.item|| itemConfig.type == (int)MergeItemType.timeBooster;;
        // if(result)
        //     result = UnlockManager.IsOpen(UnlockManager.MergeUnlockType.SplitUnlockLevel);
        return result;
    }
    public TableMergeItem GetLevelItem(int type, int level = -1) // 获取体力 金币 钻石 经验 的某个等级item  level=-1 最大等级
    {
        if (GameConfigManager.Instance == null || GameConfigManager.Instance.MergeItemList == null)
            return null;
        var config = GameConfigManager.Instance.MergeItemList.Find(x => x.type == type && x.next_level == level);
        return config;
    }

    public bool IsHaveGalleryAwards(int id)
    {
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return false;
        if (config.gallery_award == null)
            return false;
        if (config.gallery_award.Length != 2)
            return false;
        var line = GameConfigManager.Instance.GetMergeLine(config.in_line);
        return line != null;
    }
}

public enum MergeItemStatus
{
    time = 3,
    bubble = 2,
    open = 1,
    locked = 0,
    box = -1,
}

public enum MergeItemType
{
    none,
    item = 1,
    box = 2,
    diamonds=3,
    energy=4,
    decoCoin=5,
    exp=6,
    timeBooster=10,//时间加速
    dailyBox=11,
    flashsaleBox=13,
    activityExp=14,
    activityKey=15,
    broom=16,
    battlepassExp=17,
    universal=18,
    split=19,
    dogCookies=20,
    climbTreeBanana=23,
    choiceChest=21,
    hamaster=100, //仓鼠    
    eatBuild=99, //可吃东西建筑    
    easter=22,//复活节
    HappyGoExp=902,//复活节
    SpeedUp=30,//单体加速
    EnergyUnlimited=31,//无限体力
    NoCd=32,//免CD
    MagicWand=33,//魔杖
    ButterFly=34,//蝴蝶
    Parrot=35,//鹦鹉
    FlowerField=36,//花田
}

public enum SubType
{
    None,
    Matreshkas, //套娃
    Count,
}

public enum ActiveCostType // 激活消耗
{
    none,
    diamonds = 1, //消耗钻石激活
    coin, // 消耗金币激活
    time_active, //时间激活被动
    time_inactive, //时间记过 主动
    /*
    1,20 代表20钻石激活；2,100代表100金币激活；3,15代表这个物品15分钟后自动激活；4,20代表这个物品需要玩家主动点击开始倒计时，20分钟后激活；
    */
}