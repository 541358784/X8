/************************************************
 * Config class : PlayerHead
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class PlayerHead
    {   
        
        // ID
        public int id ;
        
        // 图片名
        public string Icon ;
        
        // 初始状态(1：已解锁，0：未解锁)
        public int status ;
        
        // 说明
        public string info ;
        
    }
}
