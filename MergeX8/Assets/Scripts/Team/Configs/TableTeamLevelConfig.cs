// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TeamLevelConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Team
{
    public class TableTeamLevelConfig
    {   
        // ID
        public int Id { get; set; }// 升级所需金币
        public int UpPrice { get; set; }// 人数上限
        public int MaxMember { get; set; }// 商店内容
        public int ShopContentCount { get; set; }// 任务奖励加成
        public float TaskAddition { get; set; }
    }
}