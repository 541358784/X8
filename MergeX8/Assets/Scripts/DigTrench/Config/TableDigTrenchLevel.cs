/************************************************
 * Config class : DigTrenchLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.DigTrench
{
    [System.Serializable]
    public class DigTrenchLevel
    {   
        
        // ID
        public int id ;
        
        // 关卡ID
        public int levelId ;
        
        // 关卡显示图
        public string icon ;
        
        // 累积完成装修任务数
        public int unlockNodeNum ;
        
        // 关卡资源名
        public int levelResType ;
        
        // 缩放适配
        public float adaptScasle ;
        
        // 奖励ID
        public int[] rewardID ;
        
        // 奖励数量
        public int[] rewardCnt ;
        
    }
}
