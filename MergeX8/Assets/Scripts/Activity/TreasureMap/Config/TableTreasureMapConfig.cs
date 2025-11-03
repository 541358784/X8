/************************************************
 * Config class : TreasureMapConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TreasureMapConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 数量
    public int ChipCount { get; set; }// 收集奖励
    public List<int> Reward { get; set; }// 奖励数量
    public List<int> Count { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
