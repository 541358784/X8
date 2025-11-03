using System;
using System.Collections.Generic;

[System.Serializable]
public class DailyPack : TableBase
{   
    // ID
    public int Id { get; set; }// 分组ID
    public int Groupid { get; set; }// SHOPID
    public List<int> Shopid { get; set; }// 包含内容
    public List<int> Contain { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
