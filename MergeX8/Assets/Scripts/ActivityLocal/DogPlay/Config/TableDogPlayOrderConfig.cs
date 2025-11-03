using System;
using System.Collections.Generic;

[System.Serializable]
public class DogPlayOrderConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 付费类型
    public int PayType { get; set; }// 任务类型
    public List<int> OrderType { get; set; }// 狗币数量
    public List<int> CoinCount { get; set; }// 任务数量
    public List<int> OrderCount { get; set; }// 奖励类型
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardCount { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
