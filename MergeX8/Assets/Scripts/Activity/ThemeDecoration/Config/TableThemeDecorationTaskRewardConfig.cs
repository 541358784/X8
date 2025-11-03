using System;
using System.Collections.Generic;

[System.Serializable]
public class ThemeDecorationTaskRewardConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 价值上限
    public int Max_value { get; set; }// 奖励数量
    public int Output { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
