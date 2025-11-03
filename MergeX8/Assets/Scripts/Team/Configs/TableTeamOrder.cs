// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TeamOrder
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Team
{
    public class TableTeamOrder
    {   
        // ID
        public int Id { get; set; }// 支付组=
        public int PayLevelGroup { get; set; }// 玩家最小等级
        public int PlayerMin { get; set; }// 玩家最大等级
        public int PlayerMax { get; set; }// 物品1最小难度
        public int FirstMin { get; set; }// 物品1最大难度
        public int FirstMax { get; set; }// 物品2最小难度
        public int SecondMin { get; set; }// 物品2最大难度
        public int SecondMax { get; set; }// 物品3最小难度
        public int ThirdMin { get; set; }// 物品3最大难度
        public int ThirdMax { get; set; }// 奖励ID
        public List<int> RewardIds { get; set; }// 奖励数量
        public List<int> RewardNums { get; set; }
    }
}