/************************************************
 * Config class : TableDrReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableDrReward : TableBase
{   
    
    // ID
    public int id ;
    
    // 排名
    public int rank ;
    
    // 奖励类型
    public int rewardType ;
    
    // 奖励个数
    public int rewardNum ;
    


    public override int GetID()
    {
        return id;
    }
}
