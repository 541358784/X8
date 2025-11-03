/************************************************
 * Config class : GuideGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.OutsideGuide
{
    [System.Serializable]
    public class GuideGroup
    {   
        
        // #
        public int id ;
        
        // 0.默认; 1.MERGE棋盘
        public int type ;
        
        // 局内表意文字
        public string guideKey ;
        
        // 触发条件：; 0.通过关卡; 1.初次进入游戏主界面; 2.上个引导完成后; 3.获得新回忆; 4.完成关卡X回到主界面进行1次装修后; 5.点击游戏开始按钮后，进入第X关的准备状态; 6.某ID的成就可以获取时; 7.X区域的记忆奖励可以领取时; 8.消耗金币金币解锁第X个区域时
        public int triggerType ;
        
        // 参数1
        public string triggerValue1 ;
        
        // 参数2 （0等于1大于负数小于，默认等于）
        public int triggerValue2 ;
        
    }
}
