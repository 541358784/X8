using System;
using System.Collections.Generic;

[System.Serializable]
public class RecoverCoinExchangeStarConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 购买次数
    public int BuyTimes { get; set; }// 消耗金币
    public int Coin { get; set; }// 获得星星
    public int Star { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
