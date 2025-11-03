// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : RebornPackageConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableRebornPackageConfig
    {   
        // #
        public int Id { get; set; }// 奖励内容
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }// 图标
        public string CoinImage { get; set; }
    }
}