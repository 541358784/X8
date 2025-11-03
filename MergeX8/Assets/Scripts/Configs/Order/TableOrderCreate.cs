/************************************************
 * Config class : TableOrderCreate
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderCreate : TableBase
{   
    
    // 任务ID
    public int id ;
    
    // 前置任务ID; -1 没有前置
    public int[] frontTaskIds ;
    
    // 任务类型
    public int type ;
    
    // 是否是困难任务
    public int hardTask ;
    
    // 所需物品
    public int[] itemId ;
    
    // 所需物品数量
    public int[] itemNum ;
    
    // 任务奖励ID
    public int[] rewardType ;
    
    // 奖励数量
    public int[] rewardNum ;
    
    // 是否触发任务资源礼包
    public bool assist ;
    
    // NPC头像
    public int headId ;
    


    public override int GetID()
    {
        return id;
    }
}
