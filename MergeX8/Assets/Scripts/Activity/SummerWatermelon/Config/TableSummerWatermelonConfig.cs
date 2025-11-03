using System;
using System.Collections.Generic;

[System.Serializable]
public class SummerWatermelonConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 合成链ID
    public int LineId { get; set; }// 开始时给的棋子(按等级)
    public List<int> InitItems { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
