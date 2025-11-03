using System;
using System.Collections.Generic;

[System.Serializable]
public class PhotoAlbumGlobalConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间(小时)
    public int PreheatTime { get; set; }// 提前结束时间(小时)
    public int PreEndTime { get; set; }// 相册动画ID偏移
    public int PhotoOffset { get; set; }// 当期相册列表
    public List<int> PhotoList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
