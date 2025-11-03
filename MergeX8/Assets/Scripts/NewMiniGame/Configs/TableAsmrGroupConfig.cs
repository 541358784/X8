using System;
using System.Collections.Generic;

namespace DragonPlus.Config.MiniGame
{
    [System.Serializable]
    public class AsmrGroupConfig : TableBase
    {   
        // 
        public int Id { get; set; }// 图标
        public string Icon { get; set; }// 0.不固定; 1.固定; 2.混合模式
        public int FixedOrder { get; set; }// 组内STEPID
        public List<int> StepIds { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
