using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 最大狗头数量
    public int MaxDogHead { get; set; }// 最大飞盘数量
    public int MaxFrisbee { get; set; }// 飞盘增加经验值数量
    public int FrisbeeExpValue { get; set; }// 每日任务刷新时间偏移(小时)
    public int DailyTaskRefreshTimeOffset { get; set; }// 巡逻任务奖励选择次数价格(钻石)
    public int SearchTaskPickCountPrice { get; set; }// 快速结束巡逻任务价格(每小时)
    public int SearchTaskQuickFinishPrice { get; set; }// 进入睡眠时间(分钟)
    public int SleepTime { get; set; }// 鸡腿任务奖励ID
    public List<int> OrderRewardId { get; set; }// 鸡腿任务奖励数量
    public List<int> OrderRewardNum { get; set; }// 每日总奖励ID
    public List<int> DailyTaskFinalRewardId { get; set; }// 每日总奖励数量
    public List<int> DailyTaskFinalRewardNum { get; set; }// 每日任务最终奖励解锁等级
    public int DailyTaskFinalRewardUnLockLevel { get; set; }// 最终奖励需要任务数量
    public int DailyTaskFinalRewardNeedTaskCount { get; set; }// 搜寻解锁等级
    public int SearchUnLockLevel { get; set; }// 每日狗饿时间偏移(小时)
    public int DogHungryTimeOffset { get; set; }// 礼包解锁等级
    public int ThreeOneUnLockLevel { get; set; }// 三合一礼包ID
    public int ThreeOneShopId { get; set; }// 三合一折扣
    public int Discount { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
