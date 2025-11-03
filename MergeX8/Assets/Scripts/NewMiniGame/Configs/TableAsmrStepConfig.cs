using System;
using System.Collections.Generic;

namespace DragonPlus.Config.MiniGame
{
    [System.Serializable]
    public class AsmrStepConfig : TableBase
    {   
        // 
        public int Id { get; set; }// 前置步骤
        public List<int> DependStepId { get; set; }// 操作方式; 1.点击; 2.拖拽; 3.单手滑动; 4.双手滑动; 5.双手长按
        public int ActionType { get; set; }// 持续性操作需满足的条件; 滑动以屏幕宽度为一个单位; 持续时间以秒为单位
        public float RequireValue { get; set; }// 点击操作的提示类型; 1:虚线框; 2:光圈（带动画的）
        public int TipType { get; set; }// 目标
        public string Target { get; set; }// 工具路径
        public string ToolPath { get; set; }// 触发的动画路径
        public List<string> SpinePath { get; set; }// 触发的动画名字; 一个SPINE播放多个动画，使用"-"隔开
        public List<string> SpineAnimName { get; set; }// 挂载动画节点
        public string AnimatorPath { get; set; }// 挂载在根结点的动画
        public string AnimatorName { get; set; }// 开始时隐藏的物体
        public List<string> HidePaths_Enter { get; set; }// 完成时隐藏的物体
        public List<string> HidePaths_Exit { get; set; }// 开始时显示的物体
        public List<string> ShowPaths_Enter { get; set; }// 动画播放延迟隐藏
        public List<string> HidePaths_Player_with_Delay { get; set; }// 相机坐标
        public List<float> CameraPos { get; set; }// 相机大小
        public float CameraSize { get; set; }// 手指坐标
        public List<float> TipPos { get; set; }// 手指大小
        public float TipSize { get; set; }// 视频截止点
        public float VideoFrame { get; set; }// 音效
        public List<string> Sound { get; set; }// 音效延迟
        public List<float> SoundDelay { get; set; }// 不重置坐标的工具
        public List<string> ToolsKeepPos { get; set; }// SPINE动画不做第一帧强制刷新
        public bool DontUpdateFirstSpineFrame { get; set; }// 开始时入场动画节点
        public List<string> PlaySpineAnim_Enter { get; set; }// 开始时入场动画名字
        public List<string> PlaySpineAnimName_Enter { get; set; }// 入场动画结束时显示的节点
        public List<string> ShowPaths_EnterPlaySpineFinish { get; set; }// 开始擦除时隐藏的节点
        public List<string> Hide_when_start_erase { get; set; }// 开始播放SPINE动画时显示的节点
        public List<string> ShowPaths_SpinePlay { get; set; }// 入场动画开始时隐藏的节点
        public List<string> HidePaths_EnterPlaySpineBegin { get; set; }// 过渡动画操作：; 1 = 打开过场界面; 2 = 关闭过场界面
        public int TransitionAnimationType { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
