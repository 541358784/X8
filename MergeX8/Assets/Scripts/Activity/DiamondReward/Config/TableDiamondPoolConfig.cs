using System;
using System.Collections.Generic;

[System.Serializable]
public class DiamondPoolConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 所属等级
    public int Level { get; set; }// 高付费分组; 结果池
    public List<int> ResultPool { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
