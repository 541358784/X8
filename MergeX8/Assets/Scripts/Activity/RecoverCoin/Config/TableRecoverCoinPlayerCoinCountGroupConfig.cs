using System;
using System.Collections.Generic;

[System.Serializable]
public class RecoverCoinPlayerCoinCountGroupConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 存量金币范围小
    public int CoinMin { get; set; }// 存量金币范围大
    public int CoinMax { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
