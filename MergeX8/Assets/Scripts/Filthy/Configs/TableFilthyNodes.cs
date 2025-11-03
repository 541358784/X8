using System;
using System.Collections.Generic;

namespace DragonPlus.Config.Filthy
{
    [System.Serializable]
    public class FilthyNodes : TableBase
    {   
        // 挂点ID
        public int Id { get; set; }// 关卡ID
        public int LevelId { get; set; }// 默认开启
        public bool DefatultOpen { get; set; }// 后置解锁挂点
        public int NextNodeId { get; set; }// 挂点类型; 1: 普通SPINE 动画; 2: MERGE 游戏; 3: SCREW 游戏
        public int Type { get; set; }// 根据TYPE 配置不同的参数; 1: LEVELID; 2: LEVELID 
        public int Param { get; set; }// 默认动画名字
        public List<string> SpineDefaultAnims { get; set; }// 完成时动画
        public List<string> SpineFinishAnims { get; set; }// 场景默认动画
        public string ScreenDefaultAnim { get; set; }// 场景改变动画
        public string ScreenChangeAnim { get; set; }// 场景完成动画
        public string ScreenFinishAnim { get; set; }// 默认音效
        public string DefaultAudio { get; set; }// 停止音效
        public bool StopAudio { get; set; }// 完成时音效
        public string FinishAudio { get; set; }// 挂点路径
        public string NodePath { get; set; }

        public override int GetID()
        {
            return Id;
        }
    }
}
