using System;
using System.Collections.Generic;

[System.Serializable]
public class BiuBiuGlobalConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 合成链ID
    public int LineId { get; set; }// 开始时给的棋子(按等级)
    public List<int> InitItems { get; set; }// 产出内容
    public List<int> OutPut { get; set; }// 产出权重
    public List<int> Weight { get; set; }// 总权重
    public int MaxWeight { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
