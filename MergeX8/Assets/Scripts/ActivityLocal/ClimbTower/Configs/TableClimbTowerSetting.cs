// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : ClimbTowerSetting
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.ClimbTower
{
    public class TableClimbTowerSetting
    {   
        // ID
        public int Id { get; set; }// 分层
        public int PlayLevel { get; set; }// 最终奖励ID
        public List<int> RewardItem { get; set; }// 最终奖励数量
        public List<int> RewardNum { get; set; }// 付费最终奖励ID
        public List<int> PayRewardItem { get; set; }// 付费最终奖励数量
        public List<int> PayRewardNum { get; set; }// 第一次付费SHOPID
        public int FirstPayShopId { get; set; }// 付费SHOPID
        public int PayShopId { get; set; }
    }
}