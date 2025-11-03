using System;
using System.Collections.Generic;

[System.Serializable]
public class MonopolyBlockConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 格子类型; 0:分数,1:奖励,; 2:小游戏,3:卡,; 4:起始点
    public int BlockType { get; set; }// 分数
    public int Score { get; set; }// 升级后的分数
    public List<int> UpgradeScore { get; set; }// 升级的代价
    public List<int> UpgradePrice { get; set; }// 组成员
    public List<int> GroupMember { get; set; }// 满组倍数
    public float GroupMultiValue { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 地块组特效
    public string GroupEffect { get; set; }// 分层组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
