using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableCardCollectionCardPackage : TableBase
    {   
        // 卡包ID
        public int Id { get; set; }// 卡册主题ID
        public int ThemeId { get; set; }// 卡包各个占位的ID
        public List<int> ItemList { get; set; }// 卡包星级
        public int Level { get; set; }// 是否可通过重复卡牌兑换
        public bool Instore { get; set; }// 兑换所需星星数
        public int Cost { get; set; }// 卡包ICON
        public string IconSprite { get; set; }// 权重类型
        public int WeightType { get; set; }// 卡包升级
        public int UpGrade { get; set; }// 是否可在公会共享
        public bool TeamShare { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
