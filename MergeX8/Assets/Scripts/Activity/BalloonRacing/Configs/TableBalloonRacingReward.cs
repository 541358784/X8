// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : BalloonRacingReward
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.BalloonRacing
{
    public class TableBalloonRacingReward
    {   
        // ID
        public int Id { get; set; }// 排序
        public int Order { get; set; }// 完成所需数量
        public int Collect { get; set; }// 所属阶段（1正常2循环）
        public int Stage { get; set; }// 奖励
        public List<int> RewardType1 { get; set; }// 奖励
        public List<int> RewardNumber1 { get; set; }// 奖励
        public List<int> RewardType2 { get; set; }// 奖励
        public List<int> RewardNumber2 { get; set; }// 奖励
        public List<int> RewardType3 { get; set; }// 奖励
        public List<int> RewardNumber3 { get; set; }// 前几名能获得奖励
        public int RewardRank { get; set; }
    }
}