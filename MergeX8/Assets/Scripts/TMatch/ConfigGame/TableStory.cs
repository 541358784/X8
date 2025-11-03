/************************************************
 * Config class : Story
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class Story
    {   
        
        // 剧情起始ID
        public int id ;
        
        // 后置剧情ID
        public int next_id ;
        
        // 描述
        public string desc ;
        
        // 剧情类型; 0、普通对话; 1、CG
        public int story_type ;
        
        // 触发位置; 1、完成CK关卡;      参数:关卡(LEVEL) ID; 2、完成装修阶段;      参数: 阶段(STAGES) ID; 3、解锁进入新区域;      参数:区域(AREAS) ID; 4、第一次结束CG进入世界 ;      参数：ID; 5.打开春日活动主页;      参数：已完成关卡数; 6. 进入新WORLD;      参数：WORLDID
        public int triggerPosition ;
        
        // 触发参数
        public string triggerParam ;
        
        // BI配置
        public string bi ;
        
        // BI配置
        public string bi_skip ;
        
        // 人物ID; 主角：0，是用换装资源; 哈维：1; 爷爷：2; 朱迪：3; 商人：4; 啦啦队：8; 商人跟班：9; 主持人：10; 花仙子：11; 查理：12; 波莉：13; 米亚：14; 精英男：15; 茉莉：16
        public int role_id ;
        
        // 人物参数，动画名字或者图片表情后缀
        public string role_param ;
        
        // 位置; 左：1; 右：2
        public int position ;
        
        // 是否允许SKIP
        public bool skip ;
        
        // 背景图资源名
        public string bgImage ;
        
        // 剧情文字
        public string en ;
        
    }
}
