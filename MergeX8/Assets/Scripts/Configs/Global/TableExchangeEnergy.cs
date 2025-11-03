/************************************************
 * Config class : TableExchangeEnergy
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableExchangeEnergy : TableBase
{   
    
    // #
    public int id ;
    
    // 消耗的钻石最大数量
    public int consumeDiamond ;
    
    // 消耗组钻石数量
    public int[] consumeNums ;
    
    // 兑换体力数量
    public int[] exchangeNums ;
    
    // 分钟
    public int countDown ;
    


    public override int GetID()
    {
        return id;
    }
}
