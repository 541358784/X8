/************************************************
 * Config class : CoinCompetitionConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class CoinCompetitionConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
