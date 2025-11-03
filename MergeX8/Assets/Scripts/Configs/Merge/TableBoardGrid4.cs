/************************************************
 * Config class : TableBoardGrid4
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableBoardGrid4 : TableBase
{   
    
    // 
    public int id ;
    
    // 
    public int itemId ;
    
    // -1-未开启，0-锁定，1-解锁
    public int state ;
    


    public override int GetID()
    {
        return id;
    }
}
