/************************************************
 * Config class : TableAsmrGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableAsmrGroup : TableBase
{   
    
    // ID
    public int id ;
    
    // 图标
    public string icon ;
    
    // 是否固定顺序
    public int fixedOrder ;
    
    // 组内STEPID
    public int[] stepIds ;
    


    public override int GetID()
    {
        return id;
    }
}
