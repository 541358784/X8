// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmProp
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmProp
    {   
        // 道具ID
        public int Id { get; set; }// 解锁等级
        public int UnlockLevel { get; set; }// 道具多语言
        public string NameKey { get; set; }// 道具图片
        public string Image { get; set; }// 加速消耗
        public int SpeedCost { get; set; }// 加速消耗系数
        public int SpeedCoef { get; set; }
    }
}