/************************************************
 * Config class : TableWeeklyCard
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableWeeklyCard : TableBase
{   
    
    // ID
    public int id ;
    
    // SHOPID
    public int shopId ;
    
    // 购买奖励
    public int[] firstReward ;
    
    // 奖励数量
    public int[] firstRewardNum ;
    
    // 每日奖励
    public int[] everydayReward ;
    
    // 奖励数量
    public int[] everydayRewardNum ;
    
    // 奖励天数
    public int RewardDays ;
    
    // 旧版每日奖励
    public int oldReward ;
    
    // 旧版奖励数量
    public int oldCount ;
    


    public override int GetID()
    {
        return id;
    }
}
