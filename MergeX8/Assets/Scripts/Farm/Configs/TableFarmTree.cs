// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmTree
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmTree
    {   
        // 农场 果树ID
        public int Id { get; set; }// 解锁等级
        public int UnlockLevel { get; set; }// 对应的装修挂点
        public int DecoNode { get; set; }// 果树多语言
        public string NameKey { get; set; }// 果树预制体
        public string Prefab { get; set; }// 图片
        public string Image { get; set; }// 成熟时间(秒)
        public int RipeningTime { get; set; }// 购买花费
        public int BuyCost { get; set; }// 售卖价格
        public int SellPrice { get; set; }// 加速消耗
        public int SpeedCost { get; set; }// 加速消耗系数
        public int SpeedCoef { get; set; }// 产出
        public int ProductItem { get; set; }// 产出个数
        public int ProductNum { get; set; }
    }
}