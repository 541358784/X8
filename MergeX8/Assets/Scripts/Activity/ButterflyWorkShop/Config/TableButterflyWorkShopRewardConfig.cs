using System;
using System.Collections.Generic;

[System.Serializable]
public class ButterflyWorkShopRewardConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 对应奖励
    public List<int> Reward { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
