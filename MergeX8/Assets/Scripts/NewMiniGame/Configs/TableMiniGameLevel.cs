using System;
using System.Collections.Generic;

namespace DragonPlus.Config.MiniGame
{
    [System.Serializable]
    public class MiniGameLevel : TableBase
    {   
        // 
        public int Id { get; set; }// 所属章节
        public int MiniGameId { get; set; }// "ASMR类型; 1传统ASMR; 2剧情ASMR; 3点击直接完成"
        public int LevelType { get; set; }// 对应ID; MINIGAMESELECTION; ASMRLEVELCONFIG
        public int SubLevelId { get; set; }// 关卡消耗类型
        public int CostId { get; set; }// 关卡消耗数量
        public int CostCount { get; set; }// 收集度对应奖励1
        public int RewardId { get; set; }// 收集度对应奖励数量1
        public int RewardCount { get; set; }// 泡泡ICON
        public string Icon { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
