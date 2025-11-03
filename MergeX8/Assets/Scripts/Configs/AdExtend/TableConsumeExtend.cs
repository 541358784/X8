using System;
using System.Collections.Generic;

[System.Serializable]
public class ConsumeExtend : TableBase
{   
    // #
    public int Id { get; set; }// 消耗类型
    public int ConsumeType { get; set; }// 消耗值
    public int ConsumeValue { get; set; }// 显示位置 对应RV的点位
    public string PlaceId { get; set; }// 间隔时间(秒; COMMONMONETIZATIONEVENTREASONADSEPERATECOOLDOWN
    public int ShowInterval { get; set; }// 每日次数限制; COMMON_MONETIZATION_EVENT_REASON_AD_OVERDISPLAY
    public int LimitPerDay { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
