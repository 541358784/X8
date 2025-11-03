/************************************************
 * Config class : TableDecoReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableDecoReward : TableBase
{   
    
    // 当前表中的ID
    public int id ;
    
    // 房间ID
    public int Room_id ;
    
    // 装修挂点数
    public int Progress ;
    
    // 奖品ID数组
    public int[] Reward ;
    
    // 奖品数量数组
    public int[] Reward_amount ;
    


    public override int GetID()
    {
        return id;
    }
}
