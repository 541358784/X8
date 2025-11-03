using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableCardCollectionCardBook : TableBase
    {   
        // 卡集ID
        public int Id { get; set; }// 卡集主题ID
        public int ThemeId { get; set; }// 卡集图标图名
        public string IconSprite { get; set; }// 卡集名翻译表KEY
        public string Name { get; set; }// 卡集的内容，卡片ID列表
        public List<int> Cards { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }// 卡集等级
        public int Level { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
