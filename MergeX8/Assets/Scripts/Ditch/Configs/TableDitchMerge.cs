// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : DitchMerge
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Ditch
{
    public class TableDitchMerge
    {   
        // ID
        public int Id { get; set; }// 目标ID
        public int TargetMergeId { get; set; }// 棋盘ID
        public int BoardId { get; set; }
    }
}