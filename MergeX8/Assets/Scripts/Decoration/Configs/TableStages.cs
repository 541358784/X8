/************************************************
 * Config class : TableStages
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableStages : TableBase
    {   
        
        // 阶段ID
        public int id ;
        
        // 备注
        public string beizhu ;
        
        // 阶段内挂点ID
        public int[] nodes ;
        
        // 后续阶段ID
        public int nextStageId ;
        
        // 检查NPC人数CD(秒)
        public int npcCheckCD ;
        
        // 最大NPC人数(档位由低到高)
        public int[] npcCountMax ;
        
        // 阶段内解锁NPC停留点
        public int[] npcPoints ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
