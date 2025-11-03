using System;
using System.Collections.Generic;

namespace DragonPlus.Config.CardCollect
{
    [System.Serializable]
    public class TableCardCollectionTheme : TableBase
    {   
        // 卡册主题ID
        public int Id { get; set; }// 卡册主题名翻译表KEY
        public string Name { get; set; }// 卡册主题的内容，卡集ID列表
        public List<int> CardBooks { get; set; }// 卡册主题对应的卡包ID列表，关联ITEM表
        public List<int> CardPackages { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }// 4星卡包特殊处理星级
        public int Level4SameWeight { get; set; }// 5星卡包特殊处理星级
        public int Level5SameWeight { get; set; }// 卡册主题图标名
        public string IconSprite { get; set; }// 卡册主题图集名
        public string SpriteAtlas { get; set; }// 新随机逻辑
        public bool NewRandomLogic { get; set; }// 升级主题
        public int UpGradeTheme { get; set; }// 降级主题
        public int DownGradeTheme { get; set; }// 需要解锁
        public bool NeedUnLock { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
