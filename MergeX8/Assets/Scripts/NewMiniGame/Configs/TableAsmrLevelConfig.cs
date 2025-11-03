using System;
using System.Collections.Generic;

namespace DragonPlus.Config.MiniGame
{
    [System.Serializable]
    public class AsmrLevelConfig : TableBase
    {   
        // ID
        public int Id { get; set; }// 关卡ID
        public int ResId { get; set; }// 关卡内组ID（按顺序执行)
        public List<float> GroupIds { get; set; }// 相机大小(远景，近景)-进关卡
        public List<float> CameraSize_Enter { get; set; }// 相机大小(远景，近景)-结束[不好使]
        public List<float> CameraSize_Finish { get; set; }// 结束时扫光动画流程希望关闭一些节点
        public List<string> HidePaths_Finish { get; set; }// 初始化相机位置（动画提供）
        public List<float> CameraInitPos { get; set; }// 视频关
        public string VideoName { get; set; }// 背景音
        public string BgMusic { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
