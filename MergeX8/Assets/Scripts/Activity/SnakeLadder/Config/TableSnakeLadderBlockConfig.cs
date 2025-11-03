using System;
using System.Collections.Generic;

[System.Serializable]
public class SnakeLadderBlockConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 格子类型; 0:分数,1:奖励; 2:梯子,3:蛇; 4:起始点,5:结束点
    public int BlockType { get; set; }// 分数
    public int Score { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 移动步数
    public int MoveStep { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
