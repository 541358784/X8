/************************************************
 * Config class : TableNodes
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace Decoration
{
    [System.Serializable]
    public class TableNodes : TableBase
    {   
        
        // 建筑挂点ID
        public int id ;
        
        // 备注
        public string comment ;
        
        // 默认获取此挂点
        public bool defaultOwned ;
        
        // 前置挂点ID
        public int[] dependNodeIds ;
        
        // 挂点标题
        public string title ;
        
        // 任务装修界面需要
        public string icon ;
        
        // 消耗资源
        public int costId ;
        
        // 挂点消耗资源数量
        public int price ;
        
        // 奖励ID; 资源ID或MERGE道具ID或经验或体力
        public int[] rewardId ;
        
        // 奖励数量
        public int[] rewardNumber ;
        
        // 关联挂点
        public int nodeDepends ;
        
        // 默认隐藏; 前置装修完 显示自己
        public int[] viewDependNodes ;
        
        // 装修时; 需要隐藏的挂点
        public int[] hideNodes ;
        
        // 默认建筑
        public int defaultItem ;
        
        // 可替换建筑
        public int[] itemList ;
        
        // 相机聚焦值; 不填为默认; 数值越大，相机越远
        public float cameraFocus ;
        
        // 相机聚焦值PAD; 不填为默认; 数值越大，相机越远
        public float cameraFocusPad ;
        
        // 备注
        public string cameraFocusbeizhu ;
        
        // 推荐优先级(越大等级越高)
        public int suggestLevel ;
        
        // 播放完显示动画隐藏
        public bool hideAfterShow ;
        
        // 是否区域锁定加载显示
        public bool isLockShow ;
        
        // NPC配置
        public int npcConfigId ;
        
        // 点击 选中 播放TOUCH 动画
        public bool touchAnim ;
        
        // 互动ID
        public int interactLogic ;
        


        public override int GetID()
        {
            return id;
        }
    }
}
