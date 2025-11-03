/************************************************
 * Config class : BlockName
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TileMatch
{
    [System.Serializable]
    public class BlockName
    {   
        
        // 牌面ID
        public int id ;
        
        // 牌面图片名字
        public string imageName ;
        
        // 黄金牌
        public string[] goldImage ;
        
        // 编辑器用
        public string name ;
        
    }
}
