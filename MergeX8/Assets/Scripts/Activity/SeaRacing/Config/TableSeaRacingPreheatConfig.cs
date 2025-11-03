using System;
using System.Collections.Generic;

[System.Serializable]
public class SeaRacingPreheatConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 预热时间
    public int PreheatTime { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
