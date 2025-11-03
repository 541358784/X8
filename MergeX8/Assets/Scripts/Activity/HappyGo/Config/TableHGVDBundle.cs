/************************************************
 * Config class : HGVDBundle
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.HappyGo
{
    [System.Serializable]
    public class HGVDBundle
    {   
        
        // #
        public int id ;
        
        // 购买的ID; 详见SHOP表
        public int shopItemId ;
        
        // 商品名称（多语言ID）
        public string name ;
        
        // 商品描述
        public string description ;
        
        // 显示的图标
        public string icon ;
        
        // 商品ICON背景图片
        public string image ;
        
        // 商品ID列表; 参考ITMS表
        public int[] bundleItemList ;
        
        // 商品对应的数量列表
        public int[] bundleItemCountList ;
        
        // 限购次数
        public int limit ;
        
        // 类型
        public int type ;
        
        // 权益系数(显示%)
        public float labelNum ;
        
    }
}
