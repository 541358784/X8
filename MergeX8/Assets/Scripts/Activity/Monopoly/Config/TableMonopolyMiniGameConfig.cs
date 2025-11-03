using System;
using System.Collections.Generic;

[System.Serializable]
public class MonopolyMiniGameConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 打开的顺序
    public List<int> ResultList { get; set; }// 奖金列表(对应0,1,2)
    public List<int> RewardNum { get; set; }// 奖励类型列表(对应0,1,2); (积分类型为-1)
    public List<int> RewardId { get; set; }// 权重
    public int Weight { get; set; }// 分层组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
