/************************************************
 * Config class : GuideStep
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.OutsideGuide
{
    [System.Serializable]
    public class GuideStep
    {   
        
        // 引导步骤
        public int stepId ;
        
        // 引导分组
        public int groupId ;
        
        // 0-没有箭头; 1-左边有箭头; 2-右边有箭头
        public int dialogType ;
        
        // 引导文字
        public string dialogKey ;
        
        // 气泡偏移
        public int[] dialogPos ;
        
        // 角色图; -1-没有NPC; 0-博士老头; 1-护士
        public int character ;
        
        // 角色位置类型; 0-左侧; 1-右侧
        public int characterType ;
        
        // 角色偏移
        public int[] characteroffset ;
        
        // 配音
        public string voice ;
        
        // 执行操作
        public int[] actionType ;
        
        // 参数1
        public string[] actionParm ;
        
        // 结束条件:; 0-点击屏幕任意位置; 1-点击屏幕对应位置; 2-长按屏幕对应位置
        public int finishType ;
        
        // 参数1，逗号分隔表示满足一个即可
        public string finishParm ;
        
        // 是否有遮罩
        public bool hasMask ;
        
        // 遮罩是否透明
        public bool isTransparentMask ;
        
        // 箭头类型 ; 0-没有;  1-上方有箭头;  2-下方有箭头;         3-手指右下,;        4-手指右上,;         5-手指左下,;      6-手指左上,
        public int[] arrowType ;
        
        // 箭头参数; 0-点击; 1-长按
        public int[] arrowParam ;
        
        // 手指点击位置偏移; X|Y,X|Y,…
        public string[] fingerPos ;
        
        // 是否滑动
        public bool isDrag ;
        
        // 是否需要追踪; 1-需要; 0-不需要; 默认需要
        public int[] isNeedTrace ;
        
        // 是否有圈
        public bool hasCircle ;
        
        // 洞类型; 0-圆形; 1-矩形; 2.提层
        public int[] holeType ;
        
        // 目标类型; 
        public int[] targetType ;
        
        // 目标参数
        public string[] targetParam ;
        
        // 洞半径; 
        public float[] radius ;
        
        // 孔偏移; X|Y,X|Y,…
        public string[] holeOffset ;
        
        // 强制持续时间,只支持TOUCH类的,因为这种事件是主动触发的
        public float fixedDuration ;
        
    }
}
