// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : CoinShopConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableCoinShopConfig
    {   
        // #
        public int Id { get; set; }// 奖励内容
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }// 图标
        public string Image { get; set; }
    }
}