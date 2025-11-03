/************************************************
 * Config class : TableEndlessEnergyGiftBagReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableEndlessEnergyGiftBagReward : TableBase
{   
    
    // BLABLA
    public int id ;
    
    // 奖励ID
    public int[] rewardId ;
    
    // 奖励数量
    public int[] rewardNum ;
    
    // 价格（钻石，免费为0）
    public int price ;
    
    // 循环部分
    public bool repeat ;
    


    public override int GetID()
    {
        return id;
    }
}
