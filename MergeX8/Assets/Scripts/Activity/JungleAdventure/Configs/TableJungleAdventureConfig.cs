// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : JungleAdventureConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.JungleAdventure
{
    public class TableJungleAdventureConfig
    {   
        // ID STAGE阶段
        public int Id { get; set; }// 支付组
        public int PayLevelGroup { get; set; }// 积分
        public int Score { get; set; }// 最终奖励ID
        public List<int> RewardIds { get; set; }// 最终奖励数量
        public List<int> RewardNums { get; set; }// 小奖励ID
        public List<int> SmallRewardIds { get; set; }// 小奖励数量
        public List<int> SmallRewardNums { get; set; }// 宝箱展示
        public string FinalImage { get; set; }
    }
}