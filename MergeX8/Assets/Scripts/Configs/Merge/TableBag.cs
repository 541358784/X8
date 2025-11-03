/************************************************
 * Config class : TableBag
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableBag : TableBase
{   
    
    // ID
    public int id ;
    
    // 消耗类型
    public int CointType ;
    
    // 消耗
    public int CointCost ;
    


    public override int GetID()
    {
        return id;
    }
}
