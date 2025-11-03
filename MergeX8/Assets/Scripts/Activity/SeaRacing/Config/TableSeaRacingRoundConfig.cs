using System;
using System.Collections.Generic;

[System.Serializable]
public class SeaRacingRoundConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 轮次最大分数
    public int MaxScore { get; set; }// 机器人随机组配置ID
    public List<int> RobotRandomConfigId { get; set; }// 奖励配置ID
    public List<int> RewardConfigId { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
