using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetBuildingHangPointConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 挂点路径
    public string HangPointPath { get; set; }// 可替换的ITEM列表
    public List<int> ItemList { get; set; }// 默认的ITEM
    public int DefaultItem { get; set; }// 可选择替换的ITEM列表
    public List<int> SelectItemList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
