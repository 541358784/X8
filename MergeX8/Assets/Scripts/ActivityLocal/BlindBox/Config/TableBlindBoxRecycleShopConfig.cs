using System;
using System.Collections.Generic;

[System.Serializable]
public class BlindBoxRecycleShopConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 盲盒ID
    public int BoxId { get; set; }// 回收价格
    public int RecyclePrice { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
