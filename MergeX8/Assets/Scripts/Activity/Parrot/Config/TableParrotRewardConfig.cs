using System;
using System.Collections.Generic;

[System.Serializable]
public class ParrotRewardConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 分组
    public int PayLevelGroup { get; set; }// 阶段分数 总分
    public int Score { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 分组
    public int Group { get; set; }// 组内ID
    public int GroupInnerIndex { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
