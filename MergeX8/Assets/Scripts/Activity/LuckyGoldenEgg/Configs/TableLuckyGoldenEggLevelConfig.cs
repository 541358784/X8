// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : LuckyGoldenEggLevelConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.LuckyGoldenEgg
{
    public class TableLuckyGoldenEggLevelConfig
    {   
        // 编号
        public int Id { get; set; }// 棋盘阶段
        public int Level { get; set; }// 瓶子个数
        public int ItemCount { get; set; }// 大奖下限
        public int UnderLimit { get; set; }// 小奖
        public List<int> RandomReward { get; set; }// 数量
        public List<int> RandomRewardCount { get; set; }// 大奖
        public List<int> FinishReward { get; set; }// 数量
        public List<int> FinishRewardCount { get; set; }// 分层
        public int PayLevelGroup { get; set; }
    }
}