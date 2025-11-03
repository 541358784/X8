using System;
using System.Collections.Generic;

namespace DragonPlus.Config.MiniGame
{
    [System.Serializable]
    public class MiniGameChapter : TableBase
    {   
        // 章节
        public int Id { get; set; }// 章节入口图
        public string Icon { get; set; }// 章节类型; 1:卖惨; 2:普通
        public int Type { get; set; }// 关卡描述KEY
        public string DesKey { get; set; }// 主页ICON
        public string HomeBtn { get; set; }// 对应的资源章节ID
        public int ResId { get; set; }// BGM 名
        public string BgmName { get; set; }// 章节完成时，自动领取的奖励ID
        public int ChapterRewardId { get; set; }// 章节完成时，自动领取的奖励数量
        public int ChapterRewardCnt { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
