using System;
using System.Collections.Generic;

[System.Serializable]
public class MonopolyGlobalConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 预热时间
    public float PreheatTime { get; set; }// 参加活动的玩家总数
    public int MaxPlayerCount { get; set; }// 最少进入排行榜的分数
    public int LeastEnterBoardScore { get; set; }// 跳格子力度
    public float JumpPower { get; set; }// 预览中心挂点
    public int CenterDecoItem { get; set; }// 循环宝箱
    public List<int> LoopRewardBoxList { get; set; }// 骰子点数池ID
    public List<int> DicePoolId { get; set; }// 骰子点数池数量
    public List<int> DicePoolNum { get; set; }// 格子ID列表
    public List<int> BlockList { get; set; }// 三连小游戏ID随机池
    public List<int> MiniGamePool { get; set; }// 卡牌池ID
    public List<int> CardPoolId { get; set; }// 卡牌池数量
    public List<int> CardPoolNum { get; set; }// 升级次数
    public List<int> BlockUpgradeTimes { get; set; }// 小游戏倍数; 卡片倍数; 起点奖励倍数
    public List<int> BlockUpgradeRewardMultiple { get; set; }// 初始骰子数量
    public int StartDice { get; set; }// 引导扔的骰子
    public int GuideStep { get; set; }// 押注序列
    public List<int> BetList { get; set; }// 分层组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
