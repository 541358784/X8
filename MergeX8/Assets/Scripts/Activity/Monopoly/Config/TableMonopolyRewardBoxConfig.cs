using System;
using System.Collections.Generic;

[System.Serializable]
public class MonopolyRewardBoxConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 奖励类型
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 收集类型(0:骰子数)
    public int CollectType { get; set; }// 需要收集的数量
    public int CollectNum { get; set; }// 分层组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
