using System;
using System.Collections.Generic;

[System.Serializable]
public class SeaRacingRobotConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 机器人类型; 1:按照时间增长; 2:永远比玩家分低
    public int RobotType { get; set; }// 限制在玩家分数的比例; (类型2专属)
    public float ScoreLimit { get; set; }// 分组
    public int PayLevelGroup { get; set; }// 每分钟增长波动范围
    public List<int> AddRange { get; set; }// 时间波动(秒)
    public List<int> TimeRange { get; set; }// 次数波动
    public List<int> CoutRange { get; set; }// 次数波动权重
    public List<int> CoutWeight { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
