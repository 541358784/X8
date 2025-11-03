// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : BalloonRacingRobot
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.BalloonRacing
{
    public class TableBalloonRacingRobot
    {   
        // ID
        public int Id { get; set; }// 变动分数组
        public List<int> AddRange1 { get; set; }// 分数组权重
        public List<int> TimeRange1 { get; set; }// 变动分数组
        public List<int> AddRange2 { get; set; }// 分数组权重
        public List<int> TimeRange2 { get; set; }// 变动分数组
        public List<int> AddRange3 { get; set; }// 分数组权重
        public List<int> TimeRange3 { get; set; }// 变动分数组
        public List<int> AddRange4 { get; set; }// 分数组权重
        public List<int> TimeRange4 { get; set; }
    }
}