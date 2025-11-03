using System;
using System.Collections.Generic;

[System.Serializable]
public class Easter2024StoreLevelConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 商店购买项列表
    public List<int> StoreItemList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
