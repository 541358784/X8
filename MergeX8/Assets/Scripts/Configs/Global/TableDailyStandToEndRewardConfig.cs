/************************************************
 * Config class : TableDailyStandToEndRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableDailyStandToEndRewardConfig : TableBase
{   
    
    // ID
    public int id ;
    
    // 奖励ID
    public int[] rewardItem ;
    
    // 奖励数量
    public int[] rewardNum ;
    
    // 权重
    public int[] weight ;
    
    // 复活价格(钻石)
    public int rebornPrice ;
    


    public override int GetID()
    {
        return id;
    }
}
