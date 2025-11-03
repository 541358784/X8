using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetBuildingItemConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 挂点路径
    public string AssetPath { get; set; }// 挂点路径
    public int HangPoint { get; set; }// 图标名
    public string IconAssetName { get; set; }// 图集名
    public string IconAtlasName { get; set; }// 小图标名
    public string IconSmallAssetName { get; set; }// 图标名
    public string SkinName { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
