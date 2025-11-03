// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : JungleAdventureRankRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.JungleAdventure
{
    public class TableJungleAdventureRankRewardConfig
    {   
        // 编号
        public int Id { get; set; }// 最小排名
        public int RankMin { get; set; }// 最大排名
        public int RankMax { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }
    }
}