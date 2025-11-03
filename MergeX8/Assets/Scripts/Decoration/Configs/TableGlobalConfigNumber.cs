/************************************************
 * Config class : TableGlobalConfigNumber
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableGlobalConfigNumber : TableBase
    {   
        
        // ID
        public int id ;
        
        // ID
        public string key ;
        
        // VALUE
        public float value ;
        
        // 说明
        public string desc ;
        
        // 修改日志
        public string log ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
