using System;
using System.Collections.Generic;

[System.Serializable]
public class KapiTileGiftBagConfig : TableBase
{   
    // ID
    public int Id { get; set; }// SHOPID
    public int ShopId { get; set; }// 包含内容
    public List<int> Contain { get; set; }// 包含内容数量
    public List<int> ContainCount { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
