/************************************************
 * Config class : TableInteractLogic
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableInteractLogic : TableBase
    {   
        
        // ID
        public int id ;
        
        // ID
        public int elementId ;
        
        // 开始动画
        public string startAnim ;
        
        // 结束动画
        public string endAnim ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
