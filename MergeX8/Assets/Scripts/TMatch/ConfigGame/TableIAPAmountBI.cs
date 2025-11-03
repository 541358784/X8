/************************************************
 * Config class : IAPAmountBI
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class IAPAmountBI
    {   
        
        // ID
        public int id ;
        
        // BI发送条件; 1.到达某一关卡; 2通过某一关卡(关卡ID); 3玩家类型（1付费，2免费）; 4注册时长（大于）（秒）; 5注册时长（小于）（秒）; 6小于某一关卡(关卡ID)
        public int[] openType ;
        
        // BI发送条件参数
        public int[] openParam ;
        
        // BI
        public string BI ;
        
        // 付费额（美分）
        public int iap ;
        
        // 是否单次付费0=累计1=单次
        public int isTotal ;
        
        // BI累计发送次数 -1表示无限次数
        public int totalSendTimes ;
        
    }
}
