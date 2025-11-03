// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmTimeOrderConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.FarmTimeOrder
{
    public class TableFarmTimeOrderConfig
    {   
        // 订单ID 
        public int Id { get; set; }// 订单物品信息
        public List<int> RequireMents { get; set; }// 订单物品需求数量
        public List<int> RequireNums { get; set; }// 奖励ID
        public List<int> RewardIds { get; set; }// 奖励数量
        public List<int> RewardNums { get; set; }
    }
}