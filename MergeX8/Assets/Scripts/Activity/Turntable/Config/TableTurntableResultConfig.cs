/************************************************
 * Config class : TurntableResultConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TurntableResultConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 奖励ID
    public int RewardId { get; set; }// 奖励数量
    public int RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
