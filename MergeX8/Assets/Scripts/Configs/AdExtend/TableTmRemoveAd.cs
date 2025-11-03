using System;
using System.Collections.Generic;

[System.Serializable]
public class TmRemoveAd : TableBase
{   
    // #
    public int Id { get; set; }// 暂时无用
    public int GroupId { get; set; }// 礼包推送解锁条件（单位：关卡）
    public int UnlockLevel { get; set; }// 每次推送间隔时间(秒)
    public int ShowInterval { get; set; }// 每日次数限制
    public int LimitPerDay { get; set; }// 礼包ID（前面配没金币的）
    public List<int> ShopIds { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
