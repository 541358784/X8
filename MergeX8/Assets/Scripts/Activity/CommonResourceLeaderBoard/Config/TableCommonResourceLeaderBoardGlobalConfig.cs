/************************************************
 * Config class : CommonResourceLeaderBoardGlobalConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class CommonResourceLeaderBoardGlobalConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 排行榜后台关键字(初始化后不可变)
    public string KeyWord { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }// 皮肤名称
    public string SkinName { get; set; }// 收集的资源类型
    public int CollectResourceId { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
