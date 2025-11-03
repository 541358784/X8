using System;
using System.Collections.Generic;

[System.Serializable]
public class BuyResource : TableBase
{   
    // ID
    public int Id { get; set; }// 金币组  配置商城 SHOPID
    public List<int> CoinGroup { get; set; }// 金币组奖励
    public List<int> CoinReward { get; set; }// 金币图片组
    public List<string> CoinIcons { get; set; }// 钻石组
    public List<int> DiamondGroup { get; set; }// 钻石组奖励
    public List<int> DiamondRward { get; set; }// 钻石图片组
    public List<string> DiamondIcons { get; set; }// 体力组
    public List<int> EnergyGroup { get; set; }// 体力组奖励
    public List<int> EnergyReward { get; set; }// 体力图片组
    public List<string> EnergyIcons { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
