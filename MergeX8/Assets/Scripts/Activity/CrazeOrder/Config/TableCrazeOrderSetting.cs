using System;
using System.Collections.Generic;

[System.Serializable]
public class CrazeOrderSetting : TableBase
{   
    // ID
    public int Id { get; set; }// 参加时间 分钟
    public int OpenTime { get; set; }// 总奖励ID
    public List<int> RewardIds { get; set; }// 总奖励数量
    public List<int> RewardNums { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
