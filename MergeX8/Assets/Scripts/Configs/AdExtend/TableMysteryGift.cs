using System;
using System.Collections.Generic;

[System.Serializable]
public class MysteryGift : TableBase
{   
    // ID
    public int Id { get; set; }// 广告分组ID
    public int RvGroup { get; set; }// 1号位奖励
    public List<int> Reward1 { get; set; }// 1号位奖励数量
    public List<int> RewardNum1 { get; set; }// 出现权重
    public List<int> Weight1 { get; set; }// 2号位奖励【高级物品】
    public List<int> Reward2 { get; set; }// 2号位奖励数量
    public List<int> RewardNum2 { get; set; }// 出现权重
    public List<int> Weight2 { get; set; }// 3号位【资源】
    public List<int> Reward3 { get; set; }// 3号位【资源数量】
    public List<int> RewardNum3 { get; set; }// 出现权重
    public List<int> Weight3 { get; set; }// 4号位【资源】
    public List<int> Reward4 { get; set; }// 4号位【资源类型】
    public List<int> RewardNum4 { get; set; }// 出现权重
    public List<int> Weight4 { get; set; }// 星星下限
    public int StarMin { get; set; }// 星星上限
    public int StarMax { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
