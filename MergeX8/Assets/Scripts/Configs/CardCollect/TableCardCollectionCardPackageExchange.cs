using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableCardCollectionCardPackageExchange : TableBase
    {   
        // 卡包RESOURCEID
        public int Id { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
