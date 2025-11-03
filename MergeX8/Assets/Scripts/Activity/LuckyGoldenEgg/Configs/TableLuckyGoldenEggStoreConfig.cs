// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : LuckyGoldenEggStoreConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.LuckyGoldenEgg
{
    public class TableLuckyGoldenEggStoreConfig
    {   
        // ID
        public int Id { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardCount { get; set; }// SHOPID
        public int ShopId { get; set; }
    }
}