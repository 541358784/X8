using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableCardCollectionCardItem : TableBase
    {   
        // 卡牌ID
        public int Id { get; set; }// 卡册主题ID
        public int ThemeId { get; set; }// 随机组ID
        public int GroupId { get; set; }// 卡牌星级
        public int Level { get; set; }// 兑换的蓝星星数量(用于换卡包)
        public int StarValue { get; set; }// 牌面图
        public string Sprite { get; set; }// 免费权重1
        public int FreeWeight1 { get; set; }// 免费权重2
        public int FreeWeight2 { get; set; }// 付费权重1
        public int PayWeight1 { get; set; }// 付费权重2
        public int PayWeight2 { get; set; }// 卡牌名翻译表KEY
        public string Name { get; set; }// 描述文本的翻译表KEY
        public string Text { get; set; }// 合成预览
        public bool MergeView { get; set; }// 稀有度
        public int CardValue { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
