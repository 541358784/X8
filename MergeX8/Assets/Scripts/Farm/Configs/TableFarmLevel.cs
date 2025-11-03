// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmLevel
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmLevel
    {   
        // LEVEL
        public int Id { get; set; }// 升级所需经验
        public int LevelExp { get; set; }// 升级奖励ID
        public List<int> RewardIds { get; set; }// 升级奖励数量
        public List<int> RewardNums { get; set; }
    }
}