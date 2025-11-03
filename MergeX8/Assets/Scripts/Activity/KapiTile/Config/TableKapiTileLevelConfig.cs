using System;
using System.Collections.Generic;

[System.Serializable]
public class KapiTileLevelConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 通关奖励ID
    public List<int> RewardId { get; set; }// 通关奖励数量
    public List<int> RewardNum { get; set; }// 关卡列表
    public List<int> SmallLevels { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
