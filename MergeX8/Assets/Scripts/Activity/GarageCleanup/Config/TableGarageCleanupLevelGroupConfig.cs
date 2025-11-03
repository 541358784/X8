using System;
using System.Collections.Generic;

[System.Serializable]
public class GarageCleanupLevelGroupConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 等级分布[ ]
    public List<int> LevelRange { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
