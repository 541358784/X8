/************************************************
 * Config class : TableRecovery
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableRecovery : TableBase
{   
    
    // ID
    public int id ;
    
    // 回收转化内容
    public int recovery_item_task_type ;
    
    // 回收转化内容
    public int recovery_item_task_num ;
    
    // 任务替换
    public int task_replace ;
    


    public override int GetID()
    {
        return id;
    }
}
