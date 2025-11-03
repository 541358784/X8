// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TimeOrderLineConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.LimitOrderLine
{
    public class TableTimeOrderLineConfig
    {   
        // 订单ID
        public int Id { get; set; }// 最小难度
        public List<int> MinDifficulty { get; set; }// 最大难度
        public List<int> MaxDifficulty { get; set; }// 参加时间 分钟
        public int OpenTime { get; set; }// 订单物品信息
        public List<int> Requirements { get; set; }
    }
}