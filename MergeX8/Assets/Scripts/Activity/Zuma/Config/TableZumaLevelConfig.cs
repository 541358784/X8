using System;
using System.Collections.Generic;

[System.Serializable]
public class ZumaLevelConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 关卡球组合权重
    public List<int> GroupWeight { get; set; }// 颜色数量
    public List<int> ColorWeight { get; set; }// 预制体路径
    public string PrefabAsset { get; set; }// 是否为循环关
    public bool IsLoopLevel { get; set; }// 奖励类型
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 过关分数
    public int WinScore { get; set; }// 球飞行速度
    public float BallFlySpeed { get; set; }// 球插入所用帧数(越小速度越快)
    public int BallInsertFrameCount { get; set; }// 回撞速度系数
    public float HitBackSpeed { get; set; }// 被回撞速度消失所用帧数(越小回撞效果越不明显)
    public int HitSpeedDisappearFrameCount { get; set; }// 路线移动速度
    public float MoveSpeed { get; set; }// 球尺寸
    public float BallRadius { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
