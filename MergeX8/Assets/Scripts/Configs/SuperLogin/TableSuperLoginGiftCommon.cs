/************************************************
 * Config class : TableSuperLoginGiftCommon
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableSuperLoginGiftCommon : TableBase
{   
    
    // ID
    public int id ;
    
    // ID
    public string key ;
    
    // 
    public string value ;
    


    public override int GetID()
    {
        return id;
    }
}
