using System;
using System.Collections.Generic;

[System.Serializable]
public class GiftBagLinkList : TableBase
{   
    // ID
    public int Id { get; set; }// 用户分组ID
    public int UserGroup { get; set; }// 重购节点下标(-1为不重购); 注意：第一个数字是0
    public int RepeatPosition { get; set; }// 礼包链数据 GIFTBAGLINKRESOURCE
    public List<int> ListData { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
