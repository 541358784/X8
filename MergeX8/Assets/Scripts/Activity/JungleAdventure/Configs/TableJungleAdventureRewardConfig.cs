// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : JungleAdventureRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.JungleAdventure
{
    public class TableJungleAdventureRewardConfig
    {   
        // ID
        public int Id { get; set; }// 价值上限
        public int Max_value { get; set; }// 奖励数量
        public int Output { get; set; }
    }
}