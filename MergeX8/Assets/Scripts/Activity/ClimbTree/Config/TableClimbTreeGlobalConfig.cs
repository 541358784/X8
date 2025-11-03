using System;
using System.Collections.Generic;

[System.Serializable]
public class ClimbTreeGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
