/************************************************
 * Config class : TableEndlessEnergyGiftBagGlobal
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableEndlessEnergyGiftBagGlobal : TableBase
{   
    
    // BLABLA
    public int id ;
    
    // 连续未购买体力天数
    public int dayCount ;
    
    // 持续时间(小时)
    public int activeTime ;
    


    public override int GetID()
    {
        return id;
    }
}
