/************************************************
 * Config class : TableNpcConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableNpcConfig : TableBase
    {   
        
        // ID
        public int id ;
        
        // 女主INFO
        public int chiefInfoId ;
        
        // 小狗配置
        public int dogInfoId ;
        
        // 男主INFO
        public int heroInfoId ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
