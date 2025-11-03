// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : LuckyGoldenEggActivityConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.LuckyGoldenEgg
{
    public class TableLuckyGoldenEggActivityConfig
    {   
        // 编号
        public int Id { get; set; }// 预热时间（小时）
        public float PreheatTime { get; set; }// 消耗体力给的锤子数
        public int HammerCount { get; set; }// 总奖励
        public List<int> Reward { get; set; }// 数量
        public List<int> Count { get; set; }
    }
}