// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmSeed
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmSeed
    {   
        // 农场 种子ID
        public int Id { get; set; }// 解锁等级
        public int UnlockLevel { get; set; }// 种子类型
        public int Type { get; set; }// 种子多语言
        public string NameKey { get; set; }// 种子预制体
        public string Prefab { get; set; }// 种子图片
        public string Image { get; set; }// 成熟时间(秒)
        public int RipeningTime { get; set; }// 购买类型
        public int BuyType { get; set; }// 购买花费
        public int BuyCost { get; set; }// 售卖价格
        public int SellPrice { get; set; }// 加速消耗钻石数
        public int SpeedCost { get; set; }// 加速%
        public int SpeedCoef { get; set; }// 产出
        public int ProductItem { get; set; }// 产出个数
        public int ProductNum { get; set; }
    }
}