using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableCardCollectionRandomGroup : TableBase
    {   
        // ID
        public int Id { get; set; }// 星级
        public int Level { get; set; }// 总牌数
        public int MaxCount { get; set; }// 下阶段数量
        public int NextLevelCount { get; set; }// 上阶段权重提升
        public float LastLevelWeightUp { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
