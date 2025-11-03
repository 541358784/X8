using System;
using System.Collections.Generic;

[System.Serializable]
public class SnakeLadderBuyTurntableConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 价格
    public int Price { get; set; }// 转盘数量
    public int TurntableCount { get; set; }// 限购次数
    public int SaleTimes { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
