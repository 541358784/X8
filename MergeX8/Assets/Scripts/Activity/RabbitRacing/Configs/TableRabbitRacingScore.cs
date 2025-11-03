// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : RabbitRacingScore
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.RabbitRacing
{
    public class TableRabbitRacingScore
    {   
        // ID
        public int Id { get; set; }// 每分钟增长波动范围
        public List<int> AddRange { get; set; }// 时间波动(秒)
        public List<int> TimeRange { get; set; }// 次数波动
        public List<int> CoutRange { get; set; }// 次数波动权重
        public List<int> CoutWeight { get; set; }
    }
}