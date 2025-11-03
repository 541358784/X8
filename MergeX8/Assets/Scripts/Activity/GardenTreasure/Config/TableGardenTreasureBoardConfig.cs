using System;
using System.Collections.Generic;

[System.Serializable]
public class GardenTreasureBoardConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 关卡配置
    public List<string> LevelConfig { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
