// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TimeOrderConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.TimeOrder
{
    public class TableTimeOrderConfig
    {   
        // 订单ID ID唯一; 用时间+日期代表ID
        public int Id { get; set; }// 分层组
        public int PayLevelGroup { get; set; }// 等级
        public int Level { get; set; }// 最小难度
        public List<int> MinDifficulty { get; set; }// 最大难度
        public List<int> MaxDifficulty { get; set; }// 奖励ID
        public List<int> RewardIds { get; set; }// 奖励数量
        public List<int> RewardNums { get; set; }// 订单物品信息
        public List<int> Requirements { get; set; }// 后置任务ID
        public List<int> PostOrderIds { get; set; }
    }
}