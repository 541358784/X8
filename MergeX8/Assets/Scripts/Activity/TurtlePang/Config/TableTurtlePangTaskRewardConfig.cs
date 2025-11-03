/************************************************
 * Config class : TurtlePangTaskRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TurtlePangTaskRewardConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 价值上限
    public int Max_value { get; set; }// 奖励数量
    public int Output { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
