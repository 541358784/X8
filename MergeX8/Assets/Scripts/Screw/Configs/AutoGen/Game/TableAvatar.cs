// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : Avatar
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Screw
{
    public class TableAvatar
    {   
        // 在ITEM表中的ID
        public int Item_id { get; set; }// 显示优先级（列表里排序，1代表默认使用的）
        public int Weights { get; set; }// 头像解锁：1）默认解锁 2）关卡解锁 3）获得解锁 4）限时解锁
        public int Unlock_type { get; set; }// 头像解锁条件参数（小于10万的代表关卡ID），其他都是各种活动限时解锁，程序端会对应枚举值      100001：战令
        public int Unlock_param { get; set; }// 点击未解锁时气泡文本多语言KEY
        public string Lock_desc { get; set; }
    }
}