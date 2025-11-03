/************************************************
 * Config class : TableNewDailyPackLevelChangeConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableNewDailyPackLevelChangeConfig : TableBase
{   
    
    // ID
    public int id ;
    
    // 上次价格变化
    public int last_price_change ;
    
    // 上次生成礼包是否付费
    public int is_pay_last_show ;
    
    // 当前礼包累计付费天数
    public int pay_times ;
    
    // 连续生成礼包未付费天数
    public int unpay_show_days ;
    
    // 价格变化
    public int price_change ;
    


    public override int GetID()
    {
        return id;
    }
}
