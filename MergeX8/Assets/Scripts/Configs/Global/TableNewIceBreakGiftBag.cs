/************************************************
 * Config class : TableNewIceBreakGiftBag
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableNewIceBreakGiftBag : TableBase
{   
    
    // ID
    public int id ;
    
    // SHOPID
    public int shopId ;
    
    // 道具ID
    public int[] itemId ;
    
    // 道具数量
    public int[] ItemNum ;
    
    // 礼包持续时间(分钟) 
    public int packageTime ;
    
    // 解锁等级
    public int unlockLevel ;
    


    public override int GetID()
    {
        return id;
    }
}
