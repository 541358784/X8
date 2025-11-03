using System;
using System.Collections.Generic;

[System.Serializable]
public class Easter2024GlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public float PreheatTime { get; set; }// 小游戏触发需要的点数
    public int LuckyPointCount { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }// 扔球间隔(秒)
    public float DropBallTimeInterval { get; set; }// 蛋移动速度
    public float BallMoveSpeed { get; set; }// 幸运篮子移动速度
    public float LuckyMoveSpeed { get; set; }// 重力系数
    public float GravityScale { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
