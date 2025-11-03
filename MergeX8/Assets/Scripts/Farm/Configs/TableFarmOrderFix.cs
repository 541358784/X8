// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmOrderFix
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmOrderFix
    {   
        // 固定订单ID
        public int Id { get; set; }// 任务点位ID; (-1特殊点位)
        public List<int> Slots { get; set; }// 解锁等级
        public int UnlockLevel { get; set; }// 订单物品信息
        public List<int> RequirementIds { get; set; }// 订单物品数量
        public List<int> RequirementsNums { get; set; }// 奖励ID
        public List<int> RewardIds { get; set; }// 奖励数量
        public List<int> RewardNums { get; set; }
    }
}