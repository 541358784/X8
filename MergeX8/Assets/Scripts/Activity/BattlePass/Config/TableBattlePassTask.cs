/************************************************
 * Config class : TableBattlePassTask
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Activity.BattlePass
{
    [System.Serializable]
    public class TableBattlePassTask : TableBase
    {   
        
        // ID
        public int id ;
        
        // 多语言内容KEY
        public string contentKey ;
        
        // 任务图标
        public string image ;
        
        // 任务类型
        public int type ;
        
        // 内容
        public int mergeid ;
        
        // 数量
        public int number ;
        
        // 难度
        public int difficulty ;
        
        // 获得积分
        public int reward ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
