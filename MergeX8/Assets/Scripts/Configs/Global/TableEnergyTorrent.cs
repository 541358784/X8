/************************************************
 * Config class : TableEnergyTorrent
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableEnergyTorrent : TableBase
{   
    
    // #
    public int id ;
    
    // 8倍能量狂潮触发体力门槛
    public int energyCount ;
    
    // 8倍能量狂潮持续时间/M
    public int limitTime ;
    


    public override int GetID()
    {
        return id;
    }
}
