using System;
using System.Collections.Generic;

[System.Serializable]
public class ParrotGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }// 预热 
    public int PreheatTime { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
