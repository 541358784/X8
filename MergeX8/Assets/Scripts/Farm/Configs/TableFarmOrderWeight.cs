// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmOrderWeight
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmOrderWeight
    {   
        // ID 
        public int Id { get; set; }// 等级
        public int Level { get; set; }// 1个物品权重
        public int OneItemWeight { get; set; }// 2个物品权重
        public int TwoItemWeight { get; set; }// 3个物品权重
        public int ThreeItemWeight { get; set; }
    }
}