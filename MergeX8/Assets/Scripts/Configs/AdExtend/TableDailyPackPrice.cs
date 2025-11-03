using System;
using System.Collections.Generic;

[System.Serializable]
public class DailyPackPrice : TableBase
{   
    // ID
    public int Id { get; set; }// GROUPID
    public List<int> Groupid { get; set; }// 起始价格位置
    public int Start_price { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
