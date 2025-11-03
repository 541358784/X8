// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : BalloonRacingSetting
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.BalloonRacing
{
    public class TableBalloonRacingSetting
    {   
        // ID
        public int Id { get; set; }// 完成任务 系数/100
        public int Order_coefficient { get; set; }// 气泡  系数/100
        public int Bubble_coefficient { get; set; }// 闪购 系数/100
        public int FlashBuy_coefficient { get; set; }
    }
}