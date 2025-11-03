/************************************************
 * Config class : TreasureHuntActivityConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TreasureHuntActivityConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间（小时）
    public float PreheatTime { get; set; }// 消耗体力给的锤子数
    public int HammerCount { get; set; }// 总奖励
    public List<int> Reward { get; set; }// 数量
    public List<int> Count { get; set; }// 锤子需要体力数量
    public List<int> Energy { get; set; }// 升档数量
    public List<int> Limit { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
