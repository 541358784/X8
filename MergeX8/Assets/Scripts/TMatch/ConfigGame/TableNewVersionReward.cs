/************************************************
 * Config class : NewVersionReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class NewVersionReward
    {   
        
        // #
        public int bundleId ;
        
        // 道具ID
        public int[] bundleItemList ;
        
        // 对应的道具数量
        public int[] bundleItemCountList ;
        
        // ANDROID版本号
        public int androidVersion ;
        
        // IOS版本号
        public int iosVersion ;
        
        // 更新描述
        public string content ;
        
    }
}
