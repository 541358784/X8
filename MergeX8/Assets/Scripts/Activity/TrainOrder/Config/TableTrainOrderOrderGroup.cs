using System;
using System.Collections.Generic;

namespace Activity.TrainOrder
{
    [System.Serializable]
    public class TrainOrderOrderGroup : TableBase
    {      
        // 序号
        public int Id { get; set; }
    
        // 任务ID
        public List<int> OrderId { get; set; }
    
        // 奖励ID,奖励数量
        public List<int> GroupReward { get; set; }
    
        

        public override int GetID()
        {
            return Id;
        }
    }
}
