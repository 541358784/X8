// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Bonus
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.ConfigHub.Ad
{
    public class Bonus
    {   
        // #
        public int Id { get; set; }// 0: 正常奖励; 1: 按顺序取 循环; 2: 随机奖励; 3:先按顺序在随机
        public int RewardFormat { get; set; }// 0:资源; 1:物品; 2:资源+物品
        public int RewardType { get; set; }// 奖励资源ID
        public List<int> Rewardid { get; set; }// 固定奖励序列/奖励数量
        public List<int> Num { get; set; }// 奖励权重
        public List<int> Weight { get; set; }
    }
}