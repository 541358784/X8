/************************************************
 * Config class : TableNewNewIceBreakPackReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableNewNewIceBreakPackReward : TableBase
{   
    
    // ID
    public int id ;
    
    // 道具ID
    public int[] itemId ;
    
    // 道具数量
    public int[] ItemNum ;
    
    // 是否免费
    public bool isFree ;
    
    // 礼包解锁时间(分钟) 
    public int unLockTime ;
    
    // 标签字
    public string tagText ;
    
    // 分层组
    public int payLevelGroup ;
    


    public override int GetID()
    {
        return id;
    }
}
