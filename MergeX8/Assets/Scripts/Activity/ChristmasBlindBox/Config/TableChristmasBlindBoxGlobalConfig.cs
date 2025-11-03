using System;
using System.Collections.Generic;

[System.Serializable]
public class ChristmasBlindBoxGlobalConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 计费点
    public int ShopId { get; set; }// 盲盒主题ID
    public int ThemeId { get; set; }// 盲盒数量
    public int BoxCount { get; set; }// 购买限制
    public int BuyLimit { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
