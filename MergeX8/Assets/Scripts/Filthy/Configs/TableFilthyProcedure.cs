using System;
using System.Collections.Generic;

namespace DragonPlus.Config.Filthy
{
    [System.Serializable]
    public class FilthyProcedure : TableBase
    {   
        // 流程ID
        public int Id { get; set; }// 关卡ID
        public int LevelId { get; set; }// 类型; 1 播放视频; 2 OBJECTACTIVE; 3 进入SCREW; 4 进入FILTHY; 5 进入MERGE
        public int ExecuteType { get; set; }// 参数
        public string ExecuteParam { get; set; }// 执行方式; 1 时间轴; 2 触发 ->按钮点击; 3 触发->关卡完毕
        public int TriggerType { get; set; }// 触发参数
        public string TriggerParam { get; set; }// BI
        public List<int> Bi { get; set; }

        public override int GetID()
        {
            return Id;
        }
        
        public FilthyProcedure(FilthyAProcedure config)
        {
            Id = config.Id;
            LevelId = config.LevelId;
            ExecuteType = config.ExecuteType;
            ExecuteParam = config.ExecuteParam;
            TriggerType = config.TriggerType;
            TriggerParam = config.TriggerParam;
            
            if(config.Bi != null)
                Bi = new List<int>(config.Bi);
        }
        
        public FilthyProcedure(FilthyBProcedure config)
        {
            Id = config.Id;
            LevelId = config.LevelId;
            ExecuteType = config.ExecuteType;
            ExecuteParam = config.ExecuteParam;
            TriggerType = config.TriggerType;
            TriggerParam = config.TriggerParam;
            
            if(config.Bi != null)
                Bi = new List<int>(config.Bi);
        }
    }
}
