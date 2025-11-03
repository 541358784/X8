/************************************************
 * Config class : TableBattlePassPackage
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Activity.BattlePass_2
{
    [System.Serializable]
    public class TableBattlePassPackage : TableBase
    {   
        
        // #
        public int id ;
        
        // 名称
        public string name ;
        
        // 礼包类型
        public int type ;
        
        // 消耗货币类型; 1.广告; 2.钻石; 3.免费获得; 4.充值; 5.金币
        public int consumeType ;
        
        // 消耗参数; 
        public int consumeAmount ;
        
        // 折扣多少(30% OFF)
        public int discount ;
        
        // 礼包内容
        public int[] content ;
        
        // 道具数量
        public int[] count ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
