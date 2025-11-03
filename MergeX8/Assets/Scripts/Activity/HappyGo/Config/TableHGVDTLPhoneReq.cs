/************************************************
 * Config class : HGVDTLPhoneReq
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.HappyGo
{
    [System.Serializable]
    public class HGVDTLPhoneReq
    {   
        
        // ID
        public int id ;
        
        // 要求数量(组)
        public int[] request ;
        
        // 随机权重
        public int weight ;
        
        // 是否第一组
        public bool first_group ;
        
    }
}
