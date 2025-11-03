// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TimeOrderLineSetting
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.LimitOrderLine
{
    public class TableTimeOrderLineSetting
    {   
        // ID
        public int Id { get; set; }// 参加时间 分钟
        public int OpenTime { get; set; }// 礼包弹出CD 分钟
        public int GiftPopTimeCD { get; set; }
    }
}