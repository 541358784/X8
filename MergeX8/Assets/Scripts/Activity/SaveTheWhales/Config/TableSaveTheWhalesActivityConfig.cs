using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveTheWhalesActivityConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 消耗体力门槛
    public int CostEnergy { get; set; }// 收集水滴数量
    public int CollectCount { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 刷新时间（天）
    public int CoolingCd { get; set; }// 收集时间限制/M
    public int LimitTime { get; set; }// 完成次数
    public int Upgrade { get; set; }// 未完成次数
    public int Downgrade { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
