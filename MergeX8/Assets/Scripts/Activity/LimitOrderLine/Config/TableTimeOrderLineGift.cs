// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TimeOrderLineGift
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.LimitOrderLine
{
    public class TableTimeOrderLineGift
    {   
        // ID
        public int Id { get; set; }// 分层组
        public int PayLevelGroup { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardCount { get; set; }// SHOPID
        public int ShopId { get; set; }
    }
}