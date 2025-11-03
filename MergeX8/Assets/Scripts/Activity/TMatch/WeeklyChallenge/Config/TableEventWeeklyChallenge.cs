/************************************************
 * Config class : EventWeeklyChallenge
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.WeeklyChallenge
{
    [System.Serializable]
    public class EventWeeklyChallenge
    {   
        
        // 第几周
        public int week ;
        
        // 开始时间戳豪秒
        public string starTimeSec ;
        
        // 结束时间戳豪秒
        public string endTimeSec ;
        
    }
}
