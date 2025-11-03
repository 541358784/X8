// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : LuckyGoldenEggSetting
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.LuckyGoldenEgg
{
    public class TableLuckyGoldenEggSetting
    {   
        // 编号
        public int Id { get; set; }// 分层
        public int PayLevel { get; set; }// 锤子需要体力数量
        public List<int> Energy { get; set; }// 升档数量
        public List<int> Limit { get; set; }
    }
}