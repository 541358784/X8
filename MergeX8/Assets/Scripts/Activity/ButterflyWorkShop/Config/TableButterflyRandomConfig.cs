using System;
using System.Collections.Generic;

[System.Serializable]
public class ButterflyRandomConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 领取顺序
    public List<int> Order { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
