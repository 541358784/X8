// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmOrderItem
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmOrderItem
    {   
        // ID
        public int Id { get; set; }// 等级范围; （>=）
        public int Level { get; set; }// 最小数量
        public int NumMin { get; set; }// 最大数量
        public int NumMax { get; set; }// 权重
        public int Weight { get; set; }// 1物品可刷点位
        public List<int> FirstSlot { get; set; }// 关联物品
        public List<int> LinkItem { get; set; }// 关联物品; 权重降低幅度
        public float LinkReduceWeight { get; set; }// 相同物品; 权重降低幅度
        public float SameReduceWeight { get; set; }
    }
}