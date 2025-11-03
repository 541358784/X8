using System;
using System.Collections.Generic;

[System.Serializable]
public class DailyBonusChest : TableBase
{   
    // ID
    public int Id { get; set; }// 道具ID
    public List<int> ItemId { get; set; }// 道具数量
    public List<int> ItemNum { get; set; }// 领奖天数
    public int Day { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
