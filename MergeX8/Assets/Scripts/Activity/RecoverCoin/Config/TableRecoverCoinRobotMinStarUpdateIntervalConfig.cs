using System;
using System.Collections.Generic;

[System.Serializable]
public class RecoverCoinRobotMinStarUpdateIntervalConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 机器人最小星星数值刷新间隔
    public int MinStarUpdateInterval { get; set; }// 权重
    public int Weight { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
