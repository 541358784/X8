using System;
using System.Collections.Generic;

[System.Serializable]
public class KapiScrewGiftBagConfig : TableBase
{   
    // ID
    public int Id { get; set; }// SHOPID
    public int ShopId { get; set; }// 道具1类型; 1固定; 2自选
    public int Type1 { get; set; }// 道具1ID
    public List<int> Item1 { get; set; }// 道具1数量
    public List<int> Count1 { get; set; }// 道具2类型; 1固定; 2自选
    public int Type2 { get; set; }// 道具2ID
    public List<int> Item2 { get; set; }// 道具2数量
    public List<int> Count2 { get; set; }// 道具3类型; 1固定; 2自选
    public int Type3 { get; set; }// 道具3ID
    public List<int> Item3 { get; set; }// 道具3数量
    public List<int> Count3 { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
