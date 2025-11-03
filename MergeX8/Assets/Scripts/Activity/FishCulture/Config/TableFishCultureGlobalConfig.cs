using System;
using System.Collections.Generic;

[System.Serializable]
public class FishCultureGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }// 预热时间(小时)
    public int PreheatTime { get; set; }// 提前结束时间(小时)
    public int PreEndTime { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
