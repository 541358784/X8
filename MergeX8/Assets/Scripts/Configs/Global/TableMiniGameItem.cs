/************************************************
 * Config class : TableMiniGameItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableMiniGameItem : TableBase
{   
    
    // ITEMID
    public int id ;
    
    // 包含的ITEMID
    public int configId ;
    
    // (2:挖沟3:鱼4:划线5:水管6:刺激)
    public int configType ;
    


    public override int GetID()
    {
        return id;
    }
}
