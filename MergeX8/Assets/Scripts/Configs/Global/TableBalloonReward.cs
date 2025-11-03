/************************************************
 * Config class : TableBalloonReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableBalloonReward : TableBase
{   
    
    // ID（第几次，最后一次持续使用）
    public int id ;
    
    // 道具ID
    public int[] rvRewardID ;
    
    // 道具数量
    public int[] rvRewardNum ;
    
    // 权重
    public int[] weight ;
    


    public override int GetID()
    {
        return id;
    }
}
