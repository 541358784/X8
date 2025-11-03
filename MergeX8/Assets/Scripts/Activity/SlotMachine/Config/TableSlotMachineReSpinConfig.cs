/************************************************
 * Config class : SlotMachineReSpinConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class SlotMachineReSpinConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 购买次数
    public int BuyTimes { get; set; }// 价格(钻石)
    public int Price { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
