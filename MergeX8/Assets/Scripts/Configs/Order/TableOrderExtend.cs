/************************************************
 * Config class : TableOrderExtend
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderExtend : TableBase
{   
    
    // 物品ID
    public int id ;
    
    // 拓展链
    public int[] extendMergeLines ;
    


    public override int GetID()
    {
        return id;
    }
}
