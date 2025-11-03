using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetLevelConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 经验值
    public int Exp { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 狗装修奖励(没有不填或填0)
    public int RewardBuildingItem { get; set; }// 占位档（初始和最后）
    public bool PlaceHolder { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
