using System;
using System.Collections.Generic;

[System.Serializable]
public class DailyPackPriceRules : TableBase
{   
    // ID
    public int Id { get; set; }// 上次价格变化
    public int Last_price_change { get; set; }// 上次生成礼包是否付费
    public int Is_pay_last_show { get; set; }// 当前礼包累计付费次数
    public int Pay_times { get; set; }// 连续生成礼包未付费天数
    public int Unpay_show_days { get; set; }// 价格变化
    public int Price_change { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
