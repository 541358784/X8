/************************************************
 * Config class : TableConnectLineLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableConnectLineLevel : TableBase
{   
    
    // ID
    public int id ;
    
    // 关卡ID
    public int levelId ;
    
    // 关卡显示图
    public string icon ;
    
    // 累积完成装修任务数
    public int unlockNodeNum ;
    
    // 奖励ID
    public int[] rewardID ;
    
    // 奖励数量
    public int[] rewardCnt ;
    


    public override int GetID()
    {
        return id;
    }
}
