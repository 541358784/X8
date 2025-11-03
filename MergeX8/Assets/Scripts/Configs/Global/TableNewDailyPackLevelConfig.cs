/************************************************
 * Config class : TableNewDailyPackLevelConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableNewDailyPackLevelConfig : TableBase
{   
    
    // ID
    public int id ;
    
    // 礼包列表
    public int[] packageList ;
    


    public override int GetID()
    {
        return id;
    }
}
