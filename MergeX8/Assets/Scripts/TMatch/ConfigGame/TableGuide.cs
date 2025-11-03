/************************************************
 * Config class : Guide
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class Guide
    {   
        
        // ID
        public int id ;
        
        // 教程ID
        public string guideId ;
        
        // 说明
        public string desc ;
        
        // 文字指引(KEY)
        public string textGuide ;
        
        // BI
        public string bi ;
        
        // 是否存盘; (连续步骤中，; 最后一步存盘)
        public bool saveFlag ;
        
        // 存储系列教程ID组
        public string saveGroup ;
        
        // 触发位置; 1.剧情之后 参数:剧情起始ID; 2.教程之后; 3.COOKING完成返回主页后; 4.首次打开某个界面时触发 参数:CONTROLLER名; 5.重建完成界面点了继续后 参数:AREAID
        public int triggerPosition ;
        
        // 触发参数
        public string triggerParam ;
        
        // 触发限制; 0.无限制; 1.厨具等级都为1
        public int triggerLimit ;
        
        // 点击目标类型; 1.挂点; 2.主页PLAY; 3.三选1->CONFIRM; 4.挂点购买按钮; 5.三选1; 6.挂点位置长按; 7.CK升级TAB; 8.CKCELL; 9.CK升级UPGRADE按钮; 10.CK升级完成OK; 11.夏日商城标签; 12.夏日领奖按钮; 13.万圣节活动预览按钮; 14.万圣节活动返回; 15.新世界切换TOGGLE; 16.点击开始按钮; 17.点击查看可前往区域; 18.点击前往世界
        public int[] targetTypes ;
        
        // 目标参数
        public string[] targetParams ;
        
        // 只触发动作，无UI; 1.直接进入COOKING
        public int actionTrigger ;
        
        // 动作参数
        public string actionParam ;
        
        // 点击遮罩即结束本条
        public bool clickAnyWhereToFinish ;
        
        // 纯圆; (否则为椭圆形)
        public bool rectShape ;
        
        // 纯透明MASK
        public bool clearMask ;
        
        // LEFT =1,; RIGHT = 2,; TOP = 3,; BOTTOM = 4
        public int arrowPos ;
        
        // 引导角色资源名
        public string npcName ;
        
        // 0.自动计算; 1.左上; 2.UI上默认点，不用计算
        public int npcPos ;
        
    }
}
