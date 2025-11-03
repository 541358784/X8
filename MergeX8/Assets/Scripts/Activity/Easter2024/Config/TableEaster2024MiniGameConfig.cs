using System;
using System.Collections.Generic;

[System.Serializable]
public class Easter2024MiniGameConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 打开的顺序
    public List<int> ResultList { get; set; }// 奖金列表(对应0,1,2)
    public List<int> ScoreList { get; set; }// 权重
    public int Weight { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
