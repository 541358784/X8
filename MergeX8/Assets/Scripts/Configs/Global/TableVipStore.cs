/************************************************
 * Config class : TableVipStore
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableVipStore : TableBase
{   
    
    // ID
    public int id ;
    
    // VIP等级
    public int storeid ;
    
    // 可购买的ITEM
    public int buyItem ;
    
    // 每次购买个数
    public int getNum ;
    
    // 可购买的次数
    public int buyCount ;
    
    // 购买钻石消耗 0 免费
    public int buyCost ;
    


    public override int GetID()
    {
        return id;
    }
}
