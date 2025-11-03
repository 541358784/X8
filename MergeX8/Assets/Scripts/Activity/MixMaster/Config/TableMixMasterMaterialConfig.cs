using System;
using System.Collections.Generic;

[System.Serializable]
public class MixMasterMaterialConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 坑ID
    public List<int> PoolIndex { get; set; }// 每份材料数量
    public int Count { get; set; }// 材料名翻译表
    public string NameKey { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
