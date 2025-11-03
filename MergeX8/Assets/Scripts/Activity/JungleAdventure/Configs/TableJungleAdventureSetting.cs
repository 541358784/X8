// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : JungleAdventureSetting
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.JungleAdventure
{
    public class TableJungleAdventureSetting
    {   
        // ID
        public int Id { get; set; }// 预热时间 分钟
        public int PreOpenTime { get; set; }// 进入排行榜分数
        public int EnterRankScore { get; set; }// 排行榜总人数
        public int RankPeople { get; set; }
    }
}