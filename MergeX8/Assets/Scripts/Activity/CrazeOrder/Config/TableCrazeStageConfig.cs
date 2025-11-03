using System;
using System.Collections.Generic;

[System.Serializable]
public class CrazeStageConfig : TableBase
{   
    // 阶段1-5
    public int Id { get; set; }// 分层组
    public int PayLevelGroup { get; set; }// 完成的订单个数
    public int OrderNum { get; set; }// 奖励ID
    public List<int> RewardIds { get; set; }// 奖励数量
    public List<int> RewardNums { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
