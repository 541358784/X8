// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmTimeOrderGroup
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.FarmTimeOrder
{
    public class TableFarmTimeOrderGroup
    {   
        // ID
        public int Id { get; set; }// ≤等级
        public int Level { get; set; }// 任务IDS
        public List<int> OrderIds { get; set; }// 参加时间 秒
        public int OpenTime { get; set; }
    }
}