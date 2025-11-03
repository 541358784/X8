// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TipRewardSetting
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.TipReward
{
    public class TableTipRewardSetting
    {   
        // ID
        public int Id { get; set; }// 弹出时间限制 秒
        public int PopTimeLimit { get; set; }// 任务金币奖励数量 ≥
        public int OrderMaxLevel { get; set; }// 金币系数/100F
        public int CoinFactor { get; set; }// 活动系数/100F
        public int ActivityFactor { get; set; }
    }
}