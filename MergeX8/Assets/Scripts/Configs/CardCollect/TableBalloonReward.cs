using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableBalloonReward : TableBase
    {   
        // ID（第几次，最后一次持续使用）
        public int Id { get; set; }// 道具ID
        public List<int> RvRewardID { get; set; }// 道具数量
        public List<int> RvRewardNum { get; set; }// 权重
        public List<int> Weight { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
