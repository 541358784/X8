/************************************************
 * Config class : BlockUnlock
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TileMatch
{
    [System.Serializable]
    public class BlockUnlock
    {   
        
        // 解锁顺序ID
        public int id ;
        
        // 块ID 对应BLOCKTYPE表
        public int blockId ;
        
        // 解锁关卡
        public int unlockLevel ;
        
        // 解锁图片
        public string blockImage ;
        
        // 解锁多语言KEY
        public string unlockKey ;
        
    }
}
