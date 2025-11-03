using System;
using System.Collections.Generic;

namespace DragonPlus.Config.Filthy
{
    [System.Serializable]
    public class FilthyASetting : TableBase
    {   
        // ID
        public int Id { get; set; }// 关卡ID
        public int LevelId { get; set; }// 资源关卡ID
        public int ResLevel { get; set; }// 关卡显示图
        public string Icon { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }// 累积完成装修任务数
        public int UnlockNodeNum { get; set; }// 引导组
        public List<int> GuideIds { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
