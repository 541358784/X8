/************************************************
 * Config class : TableStimulateSetting
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableStimulateSetting : TableBase
{   
    
    // ID
    public int id ;
    
    // 关卡ID
    public int levelId ;
    
    // 关卡显示图
    public string icon ;
    
    // 奖励ID
    public int[] rewardId ;
    
    // 奖励数量
    public int[] rewardNum ;
    
    // 累积完成装修任务数
    public int unlockNodeNum ;
    


    public override int GetID()
    {
        return id;
    }
}
