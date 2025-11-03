/************************************************
 * Config class : TreasureMapActivityConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TreasureMapActivityConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 数量
    public int MapCount { get; set; }// 宝图顺序
    public List<int> MapList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
