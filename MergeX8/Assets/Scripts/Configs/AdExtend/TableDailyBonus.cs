using System;
using System.Collections.Generic;

[System.Serializable]
public class DailyBonus : TableBase
{   
    // ID
    public int Id { get; set; }// 道具ID
    public List<int> ItemId { get; set; }// 道具数量
    public List<int> ItemNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
