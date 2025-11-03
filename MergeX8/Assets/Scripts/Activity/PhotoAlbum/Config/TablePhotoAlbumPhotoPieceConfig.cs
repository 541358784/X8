using System;
using System.Collections.Generic;

[System.Serializable]
public class PhotoAlbumPhotoPieceConfig : TableBase
{   
    // ID
    public int Id { get; set; }// I组成部分
    public string Asset { get; set; }// 所属照片
    public int PhotoId { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
