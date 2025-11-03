using System;
using System.Collections.Generic;

[System.Serializable]
public class RecoverCoinRobotCountConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
