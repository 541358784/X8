using System;
using System.Collections.Generic;

[System.Serializable]
public class PillowWheelTurntableConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 转盘布局
    public List<int> TurntableResultList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
