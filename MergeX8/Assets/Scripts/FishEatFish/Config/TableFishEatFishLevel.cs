/************************************************
 * Config class : FishEatFishLevel
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.FishEatFish
{
    [System.Serializable]
    public class FishEatFishLevel
    {   
        
        // ID
        public int id ;
        
        // 关卡ID
        public int levelId ;
        
        // 关卡类型(1为基础版,2为玩法2)
        public int type ;
        
        // 关卡显示图
        public string icon ;
        
        // 累积完成装修任务数
        public int unlockNodeNum ;
        
        // 奖励ID
        public int[] rewardID ;
        
        // 奖励数量
        public int[] rewardCnt ;
        
        // 关卡资源名
        public int levelResType ;
        
        // 缩放适配
        public float adaptScasle ;
        
    }
}
