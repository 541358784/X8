// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmGround
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmGround
    {   
        // 农场 地块
        public int Id { get; set; }// 解锁等级
        public int UnlockLevel { get; set; }// 需要的种子类型
        public int Type { get; set; }// 对应的装修挂点
        public int DecoNode { get; set; }// 多语言
        public string NameKey { get; set; }// 图片
        public string Image { get; set; }
    }
}