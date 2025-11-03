using System;
using System.Collections.Generic;

[System.Serializable]
public class EasterBundle : TableBase
{   
    // #
    public int Id { get; set; }// 购买的ID; 详见SHOP表
    public int ShopItemId { get; set; }// 商品名称（多语言ID）
    public string Name { get; set; }// 商品描述
    public string Description { get; set; }// 显示的图标
    public string Icon { get; set; }// 商品ICON背景图片
    public string Image { get; set; }// 商品ID列表; 参考ITMS表
    public List<int> BundleItemList { get; set; }// 商品对应的数量列表
    public List<int> BundleItemCountList { get; set; }// 物品类型; 1=ITEMS; 2=MERGEITEMS
    public List<int> BundleItemType { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
