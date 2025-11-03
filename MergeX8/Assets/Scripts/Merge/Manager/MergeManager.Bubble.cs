using System.Text;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;

public partial class MergeManager
{
    public bool IsBubble(int index,MergeBoardEnum boardId)
    {
        if (index < 0)
            return false;
        var itemStorage = GetBoardItem(index,boardId);
        return IsBubble(itemStorage);
    }

    public bool IsBubble(StorageMergeItem itemStorage)
    {
        return itemStorage.State == 2;
    }

    public float GetOccurBubbleRate(int id) //  当前气泡产生概率=初始概率+成长概率*合成次数（没有产生气泡）
    {
        float result = 0;
        // int mergeCount = 0;
        // storageBoard.MergeCounts.TryGetValue(id, out mergeCount);
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null ||config.bubble_rate==null|| config.bubble_rate.Length < 1)
            return result;
        result = config.bubble_rate[0] / 100.0f;//+ config.bubble_rate[1] / 100.0f * mergeCount;
        return result;
    }

    public int RandomBubbleType(int id)
    {
        int result = 0;
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null)
            return result;
        int rate =config.unlcok_power_bubble==null?0:config.unlcok_power_bubble.GetRandomWithPower();
        ;
        result = rate;
        return result;
    }

    public void SetBoardItemBubbleType(int type, int index,MergeBoardEnum boardId)
    {
        var boradItem = GetBoardItem(index,boardId);
        boradItem.BubbleType = type;
        // if(type == 1) 
        //     HotelWorldGuideManager.Instance.DoCheckMergeBubbleGuide(true);
    }

    System.Random random = new System.Random();

    public bool IsCanOccurBubble(int id)
    {
        bool result = false;
        float randomRate = random.Next(0, 100) / 100.0f;
        float occurRate = GetOccurBubbleRate(id);
        result = randomRate < occurRate;
        if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
        {
            StringBuilder budle = new StringBuilder();
            budle.Append("randomRate:");
            budle.Append(randomRate.ToString());
            budle.Append(",occurRate:");
            budle.Append(occurRate.ToString());
            SROptions.Current.bubbleCreateInfo = budle.ToString();

            if (result == false)
            {
                SROptions.Current.bubbleFaileInfo = "随机未命中";
            }
            
            //Debug面板设置过
            if (SROptions.Current.isSetBubblRateOpen)
            {
                return (SROptions.Current.isOpenBubblRate);
            }
        }
        return result;
    }

    public int GetBubbleLeftCdTime(int index,MergeBoardEnum boardId) // 气泡总时间60s
    {
        int result = 0;
        var item = GetBoardItem(index,boardId);
        var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
        if (item == null || item.State != 2 || itemConfig == null)
            return result;
        if (item.IsPause)
            return item.PauseCDTime;
        ulong severTime = APIManager.Instance.GetServerTime() / 1000;
        ulong openTime = item.OpenTime;
        int offset = (int) (severTime - openTime);
        result = offset > BubbleCD ? 0 : BubbleCD - offset;
        return result;
    }

    public void RefreshBubbleInfo(bool force = false)
    {
        bool isSameDay = Utils.IsSameDay((long) APIManager.Instance.GetServerTime() / 1000,
            (long) storageBoard.LastProductBubbleTime);
        if (!isSameDay || force)
        {
            storageBoard.TodayBubbleCount = 0;
            storageBoard.TodayBubbleInfo.Clear();
        }
    }

    public void RecordBubbleInfo(int id)
    {
        RefreshBubbleInfo();
        storageBoard.TodayBubbleCount += 1;
        storageBoard.LastProductBubbleTime = APIManager.Instance.GetServerTime() / 1000;
        if (storageBoard.TodayBubbleInfo.ContainsKey(id))
        {
            storageBoard.TodayBubbleInfo[id] = storageBoard.TodayBubbleInfo[id] + 1;
        }
        else
        {
            storageBoard.TodayBubbleInfo[id] = 1;
        }
    }

    public void AddToadyBubbuleCount()
    {
        storageBoard.TodayBubbleCount += 1;
    }

    public bool TodayIsCanProductBubble(int id)
    {
        bool result = false;
        result = !IsArrivalMaxBubbleCount(id) && !IsProductBubbleInCd();
        if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
        {
            if (IsArrivalMaxBubbleCount(id))
            {
                SROptions.Current.bubbleFaileInfo = "今日已到达最大数量 | 单个气泡到达上限";
            }
            else if (IsProductBubbleInCd())
            {
                SROptions.Current.bubbleFaileInfo = "两次气泡CD中";
            }
            //Debug面板设置过
            if (SROptions.Current.isSetBubblRateOpen)
            {
                return (SROptions.Current.isOpenBubblRate);
            }
        }
        return result;
    }

    private bool IsArrivalMaxBubbleCount(int id) // 单个气泡产出是否达到上限（包含今日产出是否达到上限）
    {
        bool result = true;
        if (IsArrivalTodayBubbleMaxCount())
            return result;
        var config = GameConfigManager.Instance.GetItemConfig(id);
        if (config == null || config.day_bubble_count<=0)
            return result;
        if (storageBoard.TodayBubbleInfo.ContainsKey(id))
        {
            result = storageBoard.TodayBubbleInfo[id] >= config.day_bubble_count;
        }
        else
        {
            result = false;
        }

        return result;
    }

    private bool IsArrivalTodayBubbleMaxCount() // 今日产出气泡量 是否达到上限
    {
        return storageBoard.TodayBubbleCount >= GlobalConfigManager.Instance.GetNumValue("TodayMaxBubbleCount");
    }

    private bool IsProductBubbleInCd() // 产出气泡是否在cd
    {
        bool result = true;
        ulong serverTime = APIManager.Instance.GetServerTime() / 1000;
        if (serverTime < storageBoard.LastProductBubbleTime)
        {
            result = false;
        }
        else
        {
            var offset = (int) (serverTime - storageBoard.LastProductBubbleTime);
            // offset
            result = offset < GlobalConfigManager.Instance.GetNumValue("ProductBubbleCD");
        }

        return result;
    }
    public int GetCreateBubbleCD()
    {
        ulong serverTime = APIManager.Instance.GetServerTime() / 1000;
        if (serverTime < storageBoard.LastProductBubbleTime)
        {
            return 0;
        }
        else
        {
            var  offset = (int)(serverTime - storageBoard.LastProductBubbleTime);
            int value = GlobalConfigManager.Instance.GetNumValue("ProductBubbleCD") - offset;
            if (value < 0)
                value = 0;
                
            return value;
        }
    }
    //debug
    public void DebugClearTodayBubbleInfo()
    {
        RefreshBubbleInfo(true);
    }

    public bool IsHaveOpenBox(MergeBoardEnum boardId) // 是否有正在开启的宝箱
    {
        bool result = false;
        for (int i = 0; i < storageBoard.Items.Count; i++)
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(storageBoard.Items[i].Id);
            ActiveCostType type = GetActiveCostType(itemConfig);
            if (type == ActiveCostType.time_inactive)
            {
                int leftCd = GetLeftActiveTime(i,boardId);
                if (leftCd > 0)
                    result = true;
            }
        }

        return result;
    }
}

public enum RefreshItemSource
{
    none,
    remove,
    product, //产出
    timeProduct, // 时间产出
    unlock, // 解锁格子
    rewards, // 仓库来源
    bag, //背包
    deathAdd, //死亡建筑产出
    merge,
    notDeal, // 暂不处理
    mergeOk, //普通合成
    mergeOk_omnipoten, //万能道具合成
    mergeBubble,
    exp, //经验
    webUnlock, // 钻石解锁蛛网
    newGuide, // 新手引导
}