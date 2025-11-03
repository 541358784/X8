// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : DitchBoard
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Ditch
{
    public class TableDitchBoard
    {   
        // ID
        public int Id { get; set; }// 棋盘ID
        public int BoardId { get; set; }// MERGEID 
        public int ItemId { get; set; }
    }
}