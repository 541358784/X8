/************************************************
 * Config class : TablePayLevelGlobalConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TablePayLevelGlobalConfig : TableBase
{   
    
    // ID
    public int id ;
    
    // 初始付费等级
    public int StartLevel ;
    
    // 付费后的最低等级
    public int MinLevelAfterPay ;
    
    // 新每日组ID
    public int[] NewDailyPackGroup ;
    
    // 新每日组对应付费等级
    public int[] NewDailyPackPayLevel ;
    
    // 每日组ID
    public int[] DailyPackGroup ;
    
    // 每日组对应付费等级
    public int[] DailyPackPayLevel ;
    


    public override int GetID()
    {
        return id;
    }
}
