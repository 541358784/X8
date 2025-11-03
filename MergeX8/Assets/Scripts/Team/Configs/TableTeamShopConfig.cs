// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TeamShopConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Team
{
    public class TableTeamShopConfig
    {   
        // ID
        public int Id { get; set; }// 解锁需要公会等级
        public int RequireLevel { get; set; }// 物品ID
        public int RewardId { get; set; }// 物品数量
        public int RewardCount { get; set; }// 购买价格
        public int Price { get; set; }// 可购买次数
        public int BuyTimes { get; set; }
    }
}