/************************************************
 * Config class : DecoWorld
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class DecoWorld
    {   
        
        // 编号
        public int id ;
        
        // 名字
        public string name ;
        
        // 指引节点
        public int node ;
        
        // 完成星星消耗
        public int finishStar ;
        
        // 完成图片
        public string finishIcon ;
        
        // 完成提示
        public string finishTip ;
        
    }
}
