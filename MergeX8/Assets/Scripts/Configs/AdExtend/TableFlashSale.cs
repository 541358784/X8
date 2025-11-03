using System;
using System.Collections.Generic;

[System.Serializable]
public class FlashSale : TableBase
{   
    // ID
    public int Id { get; set; }// 广告组
    public int GroupId { get; set; }// 物品类型
    public int Type { get; set; }// 阶段（星星数）
    public int StarNum { get; set; }// 金币物品
    public List<int> CoinItem { get; set; }// 金币物品权重
    public List<int> CionWighht { get; set; }// 金币原价
    public List<int> CoinPrice { get; set; }// 每轮金币价格系数
    public List<int> CoinIncrease { get; set; }// 金币购买次数
    public int CoinTimes { get; set; }// 金币位点
    public List<int> CoinLocality { get; set; }// 钻石物品
    public List<int> GemItem { get; set; }// 钻石物品权重
    public List<int> GmeWighht { get; set; }// 钻石原价
    public List<int> GemPrice { get; set; }// 每轮钻石价格系数
    public List<int> GemIncrease { get; set; }// 钻石购买次数
    public int GemTimes { get; set; }// 钻石位点
    public List<int> GemLocality { get; set; }// 建筑碎片合成链
    public List<int> BuildingMergeline { get; set; }// 最大建筑随便点位数量
    public int MaxLocality { get; set; }// 建筑碎片出售最大次数
    public int MaxTimes { get; set; }// 困难任务物品
    public List<int> HardItem { get; set; }// 物品价格
    public List<int> HardPrice { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
