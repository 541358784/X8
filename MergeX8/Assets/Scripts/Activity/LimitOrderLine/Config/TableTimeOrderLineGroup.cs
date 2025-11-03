// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TimeOrderLineGroup
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.LimitOrderLine
{
    public class TableTimeOrderLineGroup
    {   
        // ID
        public int Id { get; set; }// 分层组
        public int PayLevelGroup { get; set; }// ≤等级
        public int Level { get; set; }// 任务IDS
        public List<int> OrderIds { get; set; }// 奖励ID
        public List<int> RewardIds { get; set; }// 奖励数量
        public List<int> RewardNums { get; set; }
    }
}