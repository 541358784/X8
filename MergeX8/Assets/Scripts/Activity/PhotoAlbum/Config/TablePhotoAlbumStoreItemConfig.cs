using System;
using System.Collections.Generic;

[System.Serializable]
public class PhotoAlbumStoreItemConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 价格
    public int Price { get; set; }// 类型 1:棋子 2:装修物品
    public int Type { get; set; }// 奖励ID; 碎片ID与照片对应
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
