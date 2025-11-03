/************************************************
 * Config class : TableOrderFix
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderFix : TableBase
{   
    
    // 订单ID
    public int id ;
    
    // 区域ID
    public int[] slots ;
    
    // 解锁等级
    public int unlockLevel ;
    
    // 订单物品信息
    public int[] requirements ;
    
    // 头像ID
    public int headId ;
    


    public override int GetID()
    {
        return id;
    }
}
