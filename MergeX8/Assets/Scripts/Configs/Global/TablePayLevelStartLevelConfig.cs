/************************************************
 * Config class : TablePayLevelStartLevelConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TablePayLevelStartLevelConfig : TableBase
{   
    
    // ID
    public int id ;
    
    // 服务器分层
    public int[] UserGroups ;
    
    // 初始付费等级
    public int StartLevel ;
    
    // 付费后的最低等级
    public int MinLevelAfterPay ;
    


    public override int GetID()
    {
        return id;
    }
}
