using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetOrderConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 分组
    public int PayLevelGroup { get; set; }// 等级
    public int Level { get; set; }// 物品最小难度
    public List<int> MinDifficultys { get; set; }// 物品最大难度
    public List<int> MaxDifficultys { get; set; }// 物品1额外过滤合成链
    public List<int> FirstFilterDiffMergeLine { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
