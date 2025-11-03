// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Levels
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableLevels
    {   
        // #
        public int Id { get; set; }// #对照组
        public int LevelId { get; set; }// #关卡限时
        public int TimeLimit { get; set; }// #FALSE=FALSE,TRUE=TURE
        public bool IsTimeLevel { get; set; }// #FALSE NORMAL, TRUE HARD
        public bool Difficulty { get; set; }
    }
}