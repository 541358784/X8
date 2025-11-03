using System;
using System.Collections.Generic;

[System.Serializable]
public class MonopolyDiceConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 步数
    public int Step { get; set; }// 权重
    public int Weight { get; set; }// 分层组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
