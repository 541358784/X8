/************************************************
 * Config class : EventType
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.OutsideGuide
{
    [System.Serializable]
    public class EventType
    {   
        
        // #
        public int id ;
        
        // 类型
        public string type ;
        
        // 参数类型; 0-默认没有参数; 1-关卡ID; 2-设备ID; 3-时间
        public int paramType ;
        
    }
}
