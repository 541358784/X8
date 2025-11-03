/************************************************
 * Config class : CoinLeaderBoardPlayerCountConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class CoinLeaderBoardPlayerCountConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少入榜分数
    public int LeastEnterBoardScore { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
