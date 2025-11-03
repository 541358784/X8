using System;
using System.Collections.Generic;

namespace Activity.TrainOrder
{
    [System.Serializable]
    public class TrainOrderOrder : TableBase
    {      
        // 序号
        public int Id { get; set; }
    
        // MERGE道具ID
        public List<int> MergeItemId { get; set; }
    
        // 金币系数
        public int Coefficient { get; set; }
    
        // 奖励ID,奖励数量
        public List<int> OrderReward { get; set; }
    
        // 额外限时(秒)，没有填0
        public List<int> ExtraLimitTime { get; set; }
    
        // 额外奖励ID，每个MERGEITEM只能配一个奖励
        public List<int> ExtraRewardId { get; set; }
    
        // 额外奖励数量
        public List<int> ExtraRewardNum { get; set; }
    
        

        public override int GetID()
        {
            return Id;
        }
    }
}
