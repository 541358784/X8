/************************************************
 * Config class : TreasureMapLimitConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TreasureMapLimitConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 下限
    public int LimitMin { get; set; }// 上限
    public int LimitMax { get; set; }// 显示上限
    public int DisplayMax { get; set; }// 获得概率
    public int Probability { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
