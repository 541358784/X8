using System;
using System.Collections.Generic;

[System.Serializable]
public class ClimbTreeConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 等级
    public int Level { get; set; }// 阶段分数 总分
    public int Score { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 收集时显示的奖励
    public int RewardShowIndex { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
