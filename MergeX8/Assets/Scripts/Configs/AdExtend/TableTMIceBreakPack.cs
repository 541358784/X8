using System;
using System.Collections.Generic;

[System.Serializable]
public class TMIceBreakPack : TableBase
{   
    // 编号
    public int Id { get; set; }// 商品ID
    public int ShopId { get; set; }// 连赢次数判断条件
    public int MinWinTimes { get; set; }// 判断条件
    public int PopCd { get; set; }// 显示折扣百分比（1-100）; 配置100表示没有折扣
    public int Discount { get; set; }// 道具ID组（固定8个，影响排版）
    public List<int> ItemIds { get; set; }// 道具数量组
    public List<int> ItemNums { get; set; }// 限购次数
    public int BuyLimit { get; set; }// 每天最多展示次数
    public int ShowDayLimit { get; set; }// 显示次数小于几次显示该礼包
    public int ShowLimit { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
