using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetSearchTaskRewardPoolConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 池子编号
    public int Pool { get; set; }// 道具ID
    public int ItemId { get; set; }// 权重
    public int Weight { get; set; }// 分组
    public int GroupId { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
