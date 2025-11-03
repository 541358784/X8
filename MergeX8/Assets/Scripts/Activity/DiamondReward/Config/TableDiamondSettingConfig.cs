using System;
using System.Collections.Generic;

[System.Serializable]
public class DiamondSettingConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 区间低
    public int LowRange { get; set; }// 区间高
    public int HighRange { get; set; }// 升级所需开启数量
    public int UpgradeCount { get; set; }// 抽奖价格
    public int Price { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
