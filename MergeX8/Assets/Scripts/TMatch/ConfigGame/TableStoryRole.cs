/************************************************
 * Config class : StoryRole
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class StoryRole
    {   
        
        // ID
        public int id ;
        
        // 角色名
        public string role_name ;
        
        // 备注
        public string des ;
        
        // SPINE资源名
        public string spine_file_name ;
        
        // 图片名
        public string pic_file_name_prefix ;
        
    }
}
