using System;
using System.Collections.Generic;

[System.Serializable]
public class NewDailyPackageExtraRewardGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 购买次数
    public int BuyTimes { get; set; }// 皮肤
    public string SkinName { get; set; }// 解锁等级
    public int UnlockLevel { get; set; }// 挂点名后缀
    public int NodeName { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
