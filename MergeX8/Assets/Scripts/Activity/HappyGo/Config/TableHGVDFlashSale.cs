/************************************************
 * Config class : HGVDFlashSale
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.HappyGo
{
    [System.Serializable]
    public class HGVDFlashSale
    {   
        
        // ID
        public int id ;
        
        // 物品类型
        public int type ;
        
        // 金币物品
        public int[] coinItem ;
        
        // 金币物品权重
        public int[] cionWighht ;
        
        // 金币原价
        public int[] coinPrice ;
        
        // 每轮金币价格系数
        public int[] coinIncrease ;
        
        // 金币购买次数
        public int coinTimes ;
        
        // 金币位点
        public int[] coinLocality ;
        
        // 钻石物品
        public int[] gemItem ;
        
        // 钻石物品权重
        public int[] gmeWighht ;
        
        // 钻石原价
        public int[] gemPrice ;
        
        // 每轮钻石价格系数
        public int[] gemIncrease ;
        
        // 钻石购买次数
        public int gemTimes ;
        
        // 钻石位点
        public int[] gemLocality ;
        
        // 建筑碎片合成链
        public int[] buildingMergeline ;
        
        // 最大建筑随便点位数量
        public int maxLocality ;
        
        // 建筑碎片出售最大次数
        public int maxTimes ;
        
        // 广告组
        public int groupId ;
        
        // 阶段（星星数）
        public int starNum ;
        
    }
}
