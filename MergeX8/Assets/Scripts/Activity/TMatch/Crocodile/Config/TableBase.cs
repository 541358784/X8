/************************************************
 * Config class : Base
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.WinStreak
{
    [System.Serializable]
    public class Base
    {   
        
        // #ID
        public int id ;
        
        // 开启等级
        public int openLevel ;
        
        // 每一轮的总时间，包括等待时间 秒
        public int turnTime ;
        
        // 挑战限时 秒
        public int timeLimit ;
        
        // 失败再次发起挑战的等待时间 秒
        public int failWaitTime ;
        
        // 挑战成功进入下一轮等待时间 秒
        public int successWaitTime ;
        
        // 机器人数量
        public int robotCnt ;
        
        // 最后3轮高于总数 则强控 
        public int[] hardControllCnt ;
        
        // 强控范围  最小值（包括）
        public int[] hardControllMin ;
        
        // 强控范围 最大值（包括）
        public int[] hardControllMax ;
        
        // 奖励ID
        public int[] RewardId ;
        
        // 奖励数量
        public int[] RewardCnt ;
        
    }
}
