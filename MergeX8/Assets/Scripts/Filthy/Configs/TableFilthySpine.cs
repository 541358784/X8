using System;
using System.Collections.Generic;

namespace DragonPlus.Config.Filthy
{
    [System.Serializable]
    public class FilthySpine : TableBase
    {   
        // ID
        public int Id { get; set; }// 关卡ID
        public int LevelId { get; set; }// 按钮图片名字
        public List<string> ButtonIcons { get; set; }// 选择正确的ICON 才会出发SPINE 动画
        public List<string> CorrectIcons { get; set; }// 选择错误ANIM
        public List<string> ErrorAnim { get; set; }// 绑定的OBJ
        public List<string> LinkObjs { get; set; }// SPINE动画名字
        public List<string> SpineAnims { get; set; }// 预制体名字
        public string PrefabName { get; set; }// SPINE路径
        public string SpinePath { get; set; }// 声音音效
        public List<string> AudioNames { get; set; }// 开始动画
        public string StartAnim { get; set; }// 开始动画音效
        public string StartAudioName { get; set; }// 默认动画名字
        public string DefaultAnim { get; set; }// 初始缩放
        public float OrthographicSize { get; set; }// 摄像机缩放
        public float CameraSize { get; set; }// 摄像机位置
        public List<float> CameraInitPos { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
