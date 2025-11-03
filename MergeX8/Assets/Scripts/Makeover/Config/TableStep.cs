/************************************************
 * Config class : TableStep
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableStep : TableBase
{   
    
    // ID
    public int id ;
    
    // 所属LEVEL
    public int levelId ;
    
    // 完成条件计数
    public int finishedCount ;
    
    // 步骤开始时触发TIMELINEID; 0:没有
    public int startTrigger ;
    
    // 步骤完成时触发TIMELINEID; 0:没有
    public int finishedTrigger ;
    
    // 步骤工具图标
    public string icon ;
    


    public override int GetID()
    {
        return id;
    }
}
