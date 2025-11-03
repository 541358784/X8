using System;
using System.Collections.Generic;

[System.Serializable]
public class ZumaGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }// 每个球消除获得的分数
    public int EachBallScore { get; set; }// 起始球数
    public int StartDice { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
