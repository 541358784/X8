/************************************************
 * Config class : TablePsychology
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TablePsychology : TableBase
{   
    
    // ID
    public int id ;
    
    // 关卡选择显示图
    public string icon ;
    
    // 累积完成装修任务数
    public int unlockNodeNum ;
    
    // 奖励ID
    public int[] rewardID ;
    
    // 奖励数量
    public int[] rewardCnt ;
    
    // 蓝格子关卡ID
    public int[] blueBlockLevelId ;
    
    //  关卡IDS
    public int[] levels ;
    
    // 解释ID
    public int[] explainIds ;
    


    public override int GetID()
    {
        return id;
    }
}
