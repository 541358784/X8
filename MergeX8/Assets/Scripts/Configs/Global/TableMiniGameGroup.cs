/************************************************
 * Config class : TableMiniGameGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableMiniGameGroup : TableBase
{   
    
    // GROUPID
    public int id ;
    
    // 包含的ITEMID
    public int[] itemList ;
    
    // 奖励ID
    public int[] rewardId ;
    
    // 奖励数量
    public int[] rewardNum ;
    
    // 页签
    public int pageIndex ;
    
    // 新用户
    public bool newPlayer ;
    


    public override int GetID()
    {
        return id;
    }
}
