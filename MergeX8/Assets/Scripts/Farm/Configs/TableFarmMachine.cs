// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : FarmMachine
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Farm
{
    public class TableFarmMachine
    {   
        // 农场 生产机器ID
        public int Id { get; set; }// 解锁等级
        public int UnlockLevel { get; set; }// 对应的装修挂点
        public int DecoNode { get; set; }// 多语言
        public string NameKey { get; set; }// 图片
        public string Image { get; set; }// 机器订单
        public List<int> MachineOrder { get; set; }
    }
}