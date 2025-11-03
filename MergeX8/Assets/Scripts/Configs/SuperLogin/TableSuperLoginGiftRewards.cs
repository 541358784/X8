/************************************************
 * Config class : TableSuperLoginGiftRewards
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableSuperLoginGiftRewards : TableBase
{   
    
    // ID
    public int id ;
    
    // 奖励1（类型；ID；数量）（类型：1货币，2道具）
    public int[] r1 ;
    
    // 奖励2（类型；ID；数量）
    public int[] r2 ;
    
    // 奖励3（类型；ID；数量）
    public int[] r3 ;
    


    public override int GetID()
    {
        return id;
    }
}
