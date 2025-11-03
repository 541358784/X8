using System;
using System.Collections.Generic;

[System.Serializable]
public class SeaRacingRobotRandomConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 变动分数组
    public List<int> Robot { get; set; }// 分数组权重
    public List<int> Weight { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
