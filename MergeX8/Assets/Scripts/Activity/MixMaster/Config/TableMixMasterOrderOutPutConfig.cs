using System;
using System.Collections.Generic;

[System.Serializable]
public class MixMasterOrderOutPutConfig : TableBase
{   
    // ID 和 TYPE 随机任务ID
    public int Id { get; set; }// 任务点位组
    public List<int> OrderGroup { get; set; }// 产物
    public List<int> MaterialList { get; set; }// 权重
    public List<int> WeightList { get; set; }// 数量
    public List<int> MaterialCount { get; set; }// 预计数量
    public List<int> MaxCountList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
