/************************************************
 * Config class : TableNewNewIceBreakPackGlobal
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableNewNewIceBreakPackGlobal : TableBase
{   
    
    // ID
    public int id ;
    
    // 持续时间(分钟)
    public int time ;
    
    // SHOPID
    public int shopId ;
    
    // 奖励列表
    public int[] rewardList ;
    
    // 分层组
    public int payLevelGroup ;
    


    public override int GetID()
    {
        return id;
    }
}
