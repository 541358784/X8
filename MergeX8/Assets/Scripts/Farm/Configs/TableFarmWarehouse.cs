// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmWarehouse
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmWarehouse
    {   
        // 农场 仓库格子ID
        public int Id { get; set; }// 格子总量
        public int OpenNum { get; set; }// 开启消耗ID
        public List<int> OpenCostIds { get; set; }// 开启消耗数量
        public List<int> OpenCostNums { get; set; }
    }
}