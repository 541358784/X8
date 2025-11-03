using System;
using System.Collections.Generic;

namespace DragonPlus.Config.Filthy
{
    [System.Serializable]
    public class FilthySetting : TableBase
    {   
        // ID
        public int Id { get; set; }// 关卡ID
        public int LevelId { get; set; }// 关卡显示图
        public int ResLevel { get; set; }// 关卡显示图
        public string Icon { get; set; }// 奖励ID
        public List<int> RewardId { get; set; }// 奖励数量
        public List<int> RewardNum { get; set; }// 累积完成装修任务数
        public int UnlockNodeNum { get; set; }
        public List<int> GuideIds { get; set; }

        public override int GetID()
        {
            return Id;
        }

        public FilthySetting()
        {
            
        }
        public FilthySetting(FilthyASetting config)
        {
            Id = config.Id;
            LevelId = config.LevelId;
            ResLevel = config.ResLevel;
            Icon = config.Icon;
            
            if(config.RewardId != null)
                RewardId = new List<int>(config.RewardId);
            
            if(config.RewardNum != null)
             RewardNum = new List<int>(config.RewardNum);
            
            UnlockNodeNum = config.UnlockNodeNum;
            
            if(config.GuideIds != null)
                GuideIds = new List<int>(config.GuideIds);
        }
        
        public FilthySetting(FilthyBSetting config)
        {
            Id = config.Id;
            LevelId = config.LevelId;
            ResLevel = config.ResLevel;
            Icon = config.Icon;
            
            if(config.RewardId != null)
                RewardId = new List<int>(config.RewardId);
            
            if(config.RewardNum != null)
                RewardNum = new List<int>(config.RewardNum);
            
            UnlockNodeNum = config.UnlockNodeNum;
            
            if(config.GuideIds != null)
                GuideIds = new List<int>(config.GuideIds);
        }
    }
}
