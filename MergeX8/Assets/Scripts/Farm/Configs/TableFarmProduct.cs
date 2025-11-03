// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmProduct
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmProduct
    {   
        // ID
        public int Id { get; set; }// 多语言
        public string NameKey { get; set; }// 图片
        public string Image { get; set; }// 售卖类型; 30 农场经验
        public int SellType { get; set; }// 售卖价格
        public int SellPrice { get; set; }// EXP ORDER奖励
        public int Award { get; set; }// 建筑PREFAB NAME
        public string PrefabName { get; set; }// 原料图片
        public string Original { get; set; }// 生产描述KEY
        public string DesKey { get; set; }
    }
}