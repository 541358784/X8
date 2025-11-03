/************************************************
 * Config class : TableChoiceChest
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableChoiceChest : TableBase
{   
    
    // ID
    public int id ;
    
    // 内容
    public int[] item ;
    


    public override int GetID()
    {
        return id;
    }
}
