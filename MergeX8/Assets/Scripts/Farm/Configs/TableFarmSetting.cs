// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmSetting
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmSetting
    {   
        // ID
        public int Id { get; set; }// 种子成熟阶段
        public List<int> SeedStages { get; set; }// 果树成熟阶段
        public List<int> TreeStages { get; set; }
    }
}