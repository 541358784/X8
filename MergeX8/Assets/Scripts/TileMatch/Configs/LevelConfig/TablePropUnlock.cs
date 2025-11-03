/************************************************
 * Config class : PropUnlock
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TileMatch
{
    [System.Serializable]
    public class PropUnlock
    {   
        
        // 道具ID;  PROP_BACK = 801, //道具撤回;  PROP_SUPERBACK = 802, //道具超级撤回;  PROP_MAGIC = 803, //道具魔法棒;  PROP_SHUFFLE = 804, //道具洗牌;  PROP_EXTEND = 805, //道具格子+1
        public int propId ;
        
        // 解锁关卡
        public int unlockLevel ;
        
    }
}
