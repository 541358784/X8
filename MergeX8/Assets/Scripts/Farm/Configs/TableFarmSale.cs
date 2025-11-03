// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmSale
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmSale
    {   
        // ID
        public int Id { get; set; }// 农场等级 <=
        public int Level { get; set; }// 普通物品
        public List<int> Items { get; set; }// 普通物品权重
        public List<int> Weights { get; set; }// 物品价格
        public List<int> Prices { get; set; }// 购买次数
        public List<int> Nums { get; set; }// 特殊物品概率
        public int SpProbability { get; set; }// 特殊普通物品
        public List<int> SpItems { get; set; }// 特殊普通物品权重
        public List<int> SpWeights { get; set; }// 特殊物品价格
        public List<int> SpPrices { get; set; }// 特殊购买次数
        public List<int> SpNums { get; set; }
    }
}