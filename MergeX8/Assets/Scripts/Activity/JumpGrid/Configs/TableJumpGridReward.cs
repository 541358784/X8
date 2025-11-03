// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : JumpGridReward
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.JumpGrid
{
    public class TableJumpGridReward
    {   
        // ID
        public int Id { get; set; }// 支付组=
        public int PayLevelGroup { get; set; }// 阶段分数 总分
        public int Score { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }// 收集时显示的奖励
        public int RewardShowIndex { get; set; }
    }
}