using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetSearchTaskConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 巡逻时间(分钟)
    public int Time { get; set; }// 消耗类型(0:体力)
    public int ConsumeType { get; set; }// 消耗数量
    public int ConsumeCount { get; set; }// 奖励数量
    public int RewardCount { get; set; }// 可选数量
    public int PickCount { get; set; }// 稀有道具数量
    public List<int> RarePropCount { get; set; }// 建筑数量
    public List<int> BuildingCount { get; set; }// 稀有道具和建筑权重
    public List<int> RarePropAndBuildingWeight { get; set; }// 稀有道具池
    public int RarePropPool { get; set; }// 建筑池
    public int BuildingPool { get; set; }// 普通道具池
    public int ItemPool { get; set; }// 宝图经验
    public int MapExp { get; set; }// 狗经验
    public int Exp { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
