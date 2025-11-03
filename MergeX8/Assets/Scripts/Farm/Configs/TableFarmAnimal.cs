// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmAnimal
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmAnimal
    {   
        // 农场 生产动物ID
        public int Id { get; set; }// 解锁等级
        public int UnlockLevel { get; set; }// 对应的装修挂点
        public int DecoNode { get; set; }// 多语言
        public string NameKey { get; set; }// 图片
        public string Image { get; set; }// 成熟时间(秒)
        public int RipeningTime { get; set; }// 产出消耗ID
        public int ProductCostId { get; set; }// 产出消耗数量
        public int ProductCostNum { get; set; }// 加速消耗钻石数
        public int SpeedCost { get; set; }// 加速%
        public int SpeedCoef { get; set; }// 产出
        public int ProductItem { get; set; }// 产出数量
        public int ProductNum { get; set; }
    }
}