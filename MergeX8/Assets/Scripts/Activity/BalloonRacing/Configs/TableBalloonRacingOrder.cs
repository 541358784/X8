// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : BalloonRacingOrder
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.BalloonRacing
{
    public class TableBalloonRacingOrder
    {   
        // 随机任务槽位点位ID 21-31
        public int Id { get; set; }// 换算系数：; 代币数量=ROUND（金币数/系数）
        public int Coefficient { get; set; }
    }
}