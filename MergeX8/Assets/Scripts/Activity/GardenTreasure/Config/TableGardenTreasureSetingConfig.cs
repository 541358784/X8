using System;
using System.Collections.Generic;

[System.Serializable]
public class GardenTreasureSetingConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 支付组
    public int PayLevelGroup { get; set; }// 预热时间 分钟
    public int PreheatTime { get; set; }// 体力消耗次数
    public int Energy { get; set; }// 初始铲子个数
    public int InitShovel { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
