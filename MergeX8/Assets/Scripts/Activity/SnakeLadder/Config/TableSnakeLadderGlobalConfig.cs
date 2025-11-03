using System;
using System.Collections.Generic;

[System.Serializable]
public class SnakeLadderGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public float PreheatTime { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }// 转盘布局
    public List<int> TurntableResultList { get; set; }// 加速时间
    public float AddSpinSpeedTime { get; set; }// 最大转速(角度/S)
    public float MaxSpinSpeed { get; set; }// 最大转速持续时间
    public float KeepMaxSpinSpeedTime { get; set; }// 回弹角度
    public float BounceBackRotation { get; set; }// 回弹时间
    public float BounceBackTime { get; set; }// 减速时间
    public float ReduceSpinSpeedTime { get; set; }// 跳格子力度
    public float JumpPower { get; set; }// 循环地图
    public List<int> LoopLevelList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
