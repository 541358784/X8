/************************************************
 * Config class : TableDailyStandToEndGlobalConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableDailyStandToEndGlobalConfig : TableBase
{   
    
    // ID
    public int id ;
    
    // 类型(0免费，1首次付费,2正常付费)
    public int type ;
    
    // 计费点
    public int shopId ;
    
    // 关卡列表
    public int[] levelList ;
    
    // 最终奖励ID
    public int[] rewardItem ;
    
    // 最终奖励数量
    public int[] rewardNum ;
    


    public override int GetID()
    {
        return id;
    }
}
