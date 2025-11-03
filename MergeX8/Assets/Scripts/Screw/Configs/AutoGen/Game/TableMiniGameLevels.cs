// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : MiniGameLevels
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableMiniGameLevels
    {   
        // #
        public int Id { get; set; }// 小游戏关卡IDS
        public List<int> LevelIds { get; set; }
    }
}