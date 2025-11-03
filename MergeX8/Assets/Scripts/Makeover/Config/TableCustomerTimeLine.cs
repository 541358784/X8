/************************************************
 * Config class : TableCustomerTimeLine
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableCustomerTimeLine : TableBase
{   
    
    // ID
    public int id ;
    
    // 执行开始时间(MS)
    public int[] excuteStartTime ;
    
    // 执行动作
    public int[] excuteActionId ;
    
    // 是否需要等待ACTION完成才算完成; (0-不需要，1-需要)
    public int[] NeedWatingActionFinished ;
    
    // 结束时是否触发完成计数
    public bool isEndTriggerStepCount ;
    
    // 完成时执行动作(多用来做特效或者归位)
    public int[] finishedExcuteActionId ;
    


    public override int GetID()
    {
        return id;
    }
}
