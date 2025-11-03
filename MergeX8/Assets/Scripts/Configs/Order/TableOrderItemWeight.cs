/************************************************
 * Config class : TableOrderItemWeight
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderItemWeight : TableBase
{   
    
    // ID 
    public int id ;
    
    // 等级
    public int level ;
    
    // 1个物品权重
    public int oneItemLevelWeight ;
    
    // 2个物品权重
    public int twoItemLevelWeight ;
    
    // 3个物品权重
    public int threeItemLevelWeight ;
    
    // 1个物品权重
    public int oneItemDiffWeight ;
    
    // 2个物品权重
    public int twoItemDiffWeight ;
    
    // 3个物品权重
    public int threeItemDiffWeight ;
    


    public override int GetID()
    {
        return id;
    }
}
