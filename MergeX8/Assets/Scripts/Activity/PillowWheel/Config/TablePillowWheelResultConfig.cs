using System;
using System.Collections.Generic;

[System.Serializable]
public class PillowWheelResultConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 奖励ID
    public int RewardId { get; set; }// 奖励数量
    public int RewardNum { get; set; }// 权重
    public int Weight { get; set; }// 等级
    public int Level { get; set; }// 解锁转动次数
    public int UnlockSpinTimes { get; set; }// 分层组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
