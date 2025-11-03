using System;
using System.Collections.Generic;

[System.Serializable]
public class CrazeOrderConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 分层组
    public int PayLevelGroup { get; set; }// 等级
    public int Level { get; set; }// 阶段
    public int Stage { get; set; }// 最小难度
    public int MinDifficulty { get; set; }// 最大难度
    public int MaxDifficulty { get; set; }// 金币系数 X/100
    public int CoinFactor { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
