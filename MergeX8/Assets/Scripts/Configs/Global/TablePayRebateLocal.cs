/************************************************
 * Config class : TablePayRebateLocal
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TablePayRebateLocal : TableBase
{   
    
    // 编号
    public int id ;
    
    // 奖励物品
    public int[] rewardID ;
    
    // 奖励物品数量
    public int[] rewardNum ;
    


    public override int GetID()
    {
        return id;
    }
}
