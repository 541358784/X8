/************************************************
 * Config class : SlotMachineResultConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class SlotMachineResultConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 图标列表(1:钻石 2:体力 3:金币 4:7 5:绿钻)
    public List<int> ResultList { get; set; }// 权重列表
    public List<int> WeightList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
