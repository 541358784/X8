/************************************************
 * Config class : TableBuyDiamondTicket
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableBuyDiamondTicket : TableBase
{   
    
    // 券ID
    public int id ;
    
    // 持续时间(分钟)
    public int activeTime ;
    
    // 额外钻石比例
    public int percent ;
    
    // 计费点ID
    public int[] shopId ;
    
    //  额外钻石数量
    public int[] diamond ;
    


    public override int GetID()
    {
        return id;
    }
}
