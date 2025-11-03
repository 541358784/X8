using System;
using System.Collections.Generic;

[System.Serializable]
public class MatreshkasConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 分层组
    public int PayLevelGroup { get; set; }// 等级
    public int Level { get; set; }// 最小难度
    public int MinDifficulty { get; set; }// 最大难度
    public int MaxDifficulty { get; set; }// 预设产出队列
    public List<int> PresetQueue { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
