using System;
using System.Collections.Generic;

[System.Serializable]
public class Easter2024LevelConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 等级至少收集的分数
    public int ScoreLimit { get; set; }// 奖池列表，从左到右，-1为卡牌
    public List<int> RewardPool { get; set; }// 三连小游戏ID随机池
    public List<int> MiniGamePool { get; set; }// 卡牌池
    public List<int> CardPool { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
