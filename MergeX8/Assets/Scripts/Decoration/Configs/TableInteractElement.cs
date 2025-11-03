/************************************************
 * Config class : TableInteractElement
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableInteractElement : TableBase
    {   
        
        // ID
        public int id ;
        
        // WORLDID
        public int worldId ;
        
        // 场景中的路径
        public string path ;
        
        // 默认动画
        public string defaultAnim ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
