using System;
using System.Collections.Generic;

[System.Serializable]
public class PhotoAlbumPhotoConfig : TableBase
{   
    // ID
    public int Id { get; set; }// I组成部分
    public List<int> Parts { get; set; }// 整ICON
    public string Icon { get; set; }// 分页商品（等级只能每次提升1）
    public List<int> PageLevels { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
