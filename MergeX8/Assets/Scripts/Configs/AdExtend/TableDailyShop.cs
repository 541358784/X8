using System;
using System.Collections.Generic;

[System.Serializable]
public class DailyShop : TableBase
{   
    // 
    public int Id { get; set; }// 广告组
    public int RvGroup { get; set; }// 商品类型
    public int Type { get; set; }// 购买的ID; 详见SHOP表
    public int ShopItemId { get; set; }// 物品ID
    public int ItemId { get; set; }// 物品名称
    public string Name { get; set; }// 库存数量
    public int Amount { get; set; }// 出售价格
    public List<int> Price { get; set; }// 价格递增系数
    public List<int> Price_idex { get; set; }// 是否上架
    public int Sold { get; set; }// 全局刷新时间（H）
    public int Refresh_time { get; set; }// 解锁等级
    public int Unlock_id { get; set; }// 钻石刷新次数
    public int Refresh_gem_times { get; set; }// 刷新价格; 1.金币; 2.钻石
    public List<int> Refresh_gem_price { get; set; }// 原价
    public List<int> Price_original { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
