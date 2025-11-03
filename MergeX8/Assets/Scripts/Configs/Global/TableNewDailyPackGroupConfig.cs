/************************************************
 * Config class : TableNewDailyPackGroupConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableNewDailyPackGroupConfig : TableBase
{   
    
    // ID
    public int id ;
    
    // 价格分层列表
    public int[] levelList ;
    
    // 起始价格位置
    public int startPrice ;
    
    // 下降最低位置
    public int endPrice ;
    


    public override int GetID()
    {
        return id;
    }
}
