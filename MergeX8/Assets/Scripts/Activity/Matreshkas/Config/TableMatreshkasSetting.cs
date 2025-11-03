using System;
using System.Collections.Generic;

[System.Serializable]
public class MatreshkasSetting : TableBase
{   
    // ID
    public int Id { get; set; }// 参加时间 分钟
    public int OpenTime { get; set; }// 奖励ID
    public List<int> RewardIds { get; set; }// 奖励数量
    public List<int> RewardNums { get; set; }// 初始给的建筑ID
    public int InitMergeId { get; set; }// 活动结束 需要回收的建筑ID
    public List<int> RecycleMergeIds { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
