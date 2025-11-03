/************************************************
 * Config class : PushNotification
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class PushNotification
    {   
        
        // #
        public int id ;
        
        // 多语言
        public string text ;
        
        // 类型 1=即时，2=19:00，3=10:00
        public int type ;
        
        // 功能类型  1 能量满;         2 召回早;         3 常规;         4 召回晚
        public int function ;
        
        // 从第一次弹出引导开始计算的天数
        public int[] registerDayLimit ;
        
        // 权重
        public int weights ;
        
    }
}
