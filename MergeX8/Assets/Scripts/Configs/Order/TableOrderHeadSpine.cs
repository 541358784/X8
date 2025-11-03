/************************************************
 * Config class : TableOrderHeadSpine
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableOrderHeadSpine : TableBase
{   
    
    // ID
    public int id ;
    
    // SPINENAME
    public string spineName ;
    
    // SPINE类型; 0 普通任务; 1 限时任务; 2 小狗
    public int type ;
    
    // 默认动画
    public string normalAnim ;
    
    // 高兴名字
    public string happyAnim ;
    


    public override int GetID()
    {
        return id;
    }
}
