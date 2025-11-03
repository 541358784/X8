using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableCardCollectionCardPackageItem : TableBase
    {   
        // 卡包内容占位ID
        public int Id { get; set; }// 产出各个等级卡牌的权重
        public List<int> Weight { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
