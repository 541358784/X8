/************************************************
 * Config class : TableOrderAppend
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderAppend : TableBase
{   
    
    // 订单ID
    public int id ;
    
    // 解锁条件; -1; 1 通过等级解锁
    public int unLockType ;
    
    // 解锁等级
    public string[] unLockParam ;
    
    // 后置任务ID
    public int[] postOrderIds ;
    
    // 订单物品信息
    public int[] requirements ;
    
    // 奖励ID
    public int[] rewardIds ;
    
    // 奖励数量
    public int[] rewardNums ;
    


    public override int GetID()
    {
        return id;
    }
}
