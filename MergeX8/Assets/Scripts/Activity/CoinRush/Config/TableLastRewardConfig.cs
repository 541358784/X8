using System;
using System.Collections.Generic;

[System.Serializable]
public class LastRewardConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 名称
    public string Name { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
