using System;
using System.Collections.Generic;

[System.Serializable]
public class BiuBiuUIConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 气球下标
    public List<int> ShowIndex { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 粒子资源名
    public string EffectAsset { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
