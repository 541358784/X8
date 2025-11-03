/************************************************
 * Config class : TableMoLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableMoLevel : TableBase
{   
    
    // ID
    public int id ;
    
    // 是否视频关卡
    public bool isVideo ;
    
    // STEPLEVELID
    public int stepLevelId ;
    
    // 关卡ID
    public int levelId ;
    
    // 子关卡ID
    public int subID ;
    
    // 关卡显示图
    public string icon ;
    
    // 累积完成装修任务数
    public int unlockNodeNum ;
    
    // 关卡收集度需求
    public int[] levelRewardsPercentage ;
    
    // 奖励ID
    public int[] rewardID ;
    
    // 奖励数量
    public int[] rewardCnt ;
    
    // 关卡资源名
    public string resName ;
    
    // 缩放适配
    public float adaptScasle ;
    
    // 新的关卡形式
    public bool newLevel ;
    
    // 关卡内组ID（按顺序执行)
    public int[] groupIds ;
    
    // 相机大小(远景，近景)
    public float[] cameraSize_Enter ;
    
    // 相机大小(远景，近景)
    public float[] cameraSize_Finish ;
    
    // 初始化相机位置
    public float[] cameraInitPos ;
    


    public override int GetID()
    {
        return id;
    }
}
