/************************************************
 * Config class : TableGlobalConfigNumber
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableGlobalConfigNumber : TableBase
{   
    
    // ID
    public int id ;
    
    // ID
    public string key ;
    
    // VALUE
    public string value ;
    
    // 说明
    public string desc ;
    


    public override int GetID()
    {
        return id;
    }
}
