// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : TeamGlobalConfig
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.Config.Team
{
    public class TableTeamGlobalConfig
    {   
        // ID
        public int Id { get; set; }// 创建队伍需要的金币
        public int CreateCoins { get; set; }// 队伍名称最短长度
        public int TeamNameMin { get; set; }// 队伍名称最大长度
        public int TeamNameMax { get; set; }// 队伍描述最大长度
        public int TeamDescMax { get; set; }// 系统消息展示有效期（小时）
        public int SystemChatTimeLimit { get; set; }// 生命值上限
        public int MaxLife { get; set; }// 生命值恢复时间(分钟)
        public int LifeRecoverTime { get; set; }// 抽卡保底
        public int CardFailTimes { get; set; }
    }
}