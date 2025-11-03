/************************************************
 * Config class : BlockType
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TileMatch
{
    [System.Serializable]
    public class BlockType
    {   
        
        // 牌面类型 用ID当作TYPE
        public int id ;
        
        // 遮罩
        public string blockMaskName ;
        
        // 编辑器用
        public string name ;
        
    }
}
