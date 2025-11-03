/************************************************
 * Config class : TableLocalPigBox
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableLocalPigBox : TableBase
{   
    
    // BLABLA
    public int id ;
    
    // 解锁等级
    public int unlockLevel ;
    
    // 持续时间(小时)
    public int activeTime ;
    


    public override int GetID()
    {
        return id;
    }
}
