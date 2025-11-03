/************************************************
 * Config class : TableMultipleScore
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableMultipleScore : TableBase
{   
    
    // ID
    public int id ;
    
    // 影响的功能; 1.美人鱼; 2.主题装修
    public int influenceFunction ;
    
    // 对应倍数
    public float multiple ;
    
    // 生效时间(小时)
    public int activeTime ;
    


    public override int GetID()
    {
        return id;
    }
}
