using System;
using System.Collections.Generic;

[System.Serializable]
public class GiftBagSendThreeList : TableBase
{   
    // ID
    public int Id { get; set; }// 用户分组ID
    public int UserGroup { get; set; }// 礼包链数据; GIFTBAGLINKRESOURCE
    public List<int> ListData { get; set; }// 折扣标签
    public string LabelText { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
