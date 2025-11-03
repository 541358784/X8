using System;
using System.Collections.Generic;

namespace Activity.TrainOrder
{
    [System.Serializable]
    public class TrainOrderLevel : TableBase
    {      
        // 序号
        public int Id { get; set; }
    
        // 分层ID
        public int GroupId { get; set; }
    
        // 建筑ID
        public List<int> BuildId { get; set; }
    
        // 任务组ID
        public List<int> OrderGroupId { get; set; }
    
        

        public override int GetID()
        {
            return Id;
        }
    }
}
