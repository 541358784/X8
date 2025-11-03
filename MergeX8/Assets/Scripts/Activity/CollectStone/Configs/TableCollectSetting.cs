// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : CollectSetting
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.CollectStone
{
    public class TableCollectSetting
    {   
        // ID
        public int Id { get; set; }// 支付等级
        public int PayLevel { get; set; }// 固定奖励序列
        public List<int> FixReward { get; set; }// 循环奖励序列
        public List<int> LoopReward { get; set; }
    }
}