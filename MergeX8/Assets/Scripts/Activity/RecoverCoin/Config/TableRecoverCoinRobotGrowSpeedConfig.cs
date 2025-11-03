using System;
using System.Collections.Generic;

[System.Serializable]
public class RecoverCoinRobotGrowSpeedConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 存量金币范围小
    public int CoinMin { get; set; }// 存量金币范围大
    public int CoinMax { get; set; }// 每种机器人的权重
    public List<int> Weight { get; set; }// 每种机器人最大星星数量下限
    public List<int> GrowSpeedMin { get; set; }// 每种机器人七天星星数量上限
    public List<int> GrowSpeedMax { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
