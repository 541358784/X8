/************************************************
 * Config class : Shop
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class Shop
    {   
        
        // #
        public int id ;
        
        // 价格
        public float price ;
        
        // 0:钻石; 1:金币; 2:打折商品; 3:BUNDLE; 4.LIMITBUNDLE; 5.小猪存钱罐; 6.新手礼包; 7.日常礼包; 8.破冰礼包; 9.失败礼包; 10.MAP启航礼包; 11.RV商城礼包; 12.战令2022万圣节; 13.战令2022圣诞节; 14.2022年圣诞节活动; 15.战令2023一月战令; 16.战令2023情人节; 17.买二赠一礼包; 18.战令圣帕特里克节2023; 19.战令2023复活节; 20.局内复活礼包; 21.复活节礼包; 22.战令2023春游; 23.MERGE礼包; 24.战令2023舞会; 25.小猪2; 26.周年庆礼包; 27.耐心排行榜礼包; 28.战令2023海滩; 29.砸蛋活动-砸贝壳礼包; 30.无尽礼包; 31.战令2023仲夏夜; 32.TM; 33.宠物礼包; 34.战令2023运动会
        public int productType ;
        
        // 0-普通商品; 1-不可消耗商品; 2-订阅
        public int purchaseType ;
        
        // 商品在GOOGLESTORE中的ID
        public string product_id ;
        
        // 商品在APPLESTORE中的ID
        public string product_id_ios ;
        
        // 商品名称
        public string name ;
        
        // 商品描述
        public string description ;
        
        // 商品图片
        public string image ;
        
        // 购买实际得到的钻石数量(控制飞的钻石数量）
        public int amount ;
        
        // 额外获得百分比
        public float discount ;
        
        // 是否在商城上架
        public bool onSale ;
        
        // 商城首页显示
        public bool onMainPage ;
        
        // 0-普通; 1-打折促销; 2-最受欢迎; 3-火爆
        public int best_deal ;
        
        // 限购次数
        public int lmtNum ;
        
        // 折扣比,仅显示用
        public float showDiscount ;
        
        // 显示的图标
        public string icon ;
        
        // 给日均付费（美分）等于多少的人推荐该礼包
        public int targetAvgCost ;
        
        // 购买需要发送的BI
        public string buyBI ;
        
    }
}
