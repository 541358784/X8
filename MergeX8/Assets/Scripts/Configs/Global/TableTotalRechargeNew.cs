/************************************************
 * Config class : TableTotalRechargeNew
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableTotalRechargeNew : TableBase
{   
    
    // ID
    public int id ;
    
    // ID
    public int payLevelGroup ;
    
    // 累计充值（美分）（钻石）
    public int score ;
    
    // 奖励ID
    public int[] rewardId ;
    
    // 奖励数量
    public int[] rewardNum ;
    
    // 建筑奖励ID（新手累充飞艇）
    public int[] decoRewardId ;
    


    public override int GetID()
    {
        return id;
    }
}
