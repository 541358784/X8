/************************************************
 * Config class : TableShop
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableShop : TableBase
{   
    
    // #
    public int id ;
    
    // 商品名称
    public string name ;
    
    // 商品描述
    public string description ;
    
    // 美元价格
    public float price ;
    
    // 商品在GOOGLEPLAY中的ID
    public string product_id ;
    
    // 商品在APPSTORE中的ID
    public string product_id_ios ;
    
    // 商品图片
    public string image ;
    
    // 商品背景图 不填为默认
    public string background ;
    
    // 购买实际得到的货币数量; AREA = 4为钻石; AREA = 3为金币
    public int amount ;
    
    // 钻石价格; 没有配置美元价格时才有效
    public int diamondPrice ;
    
    // 额外获得百分比
    public float discount ;
    
    // 所属区域; 0. SHOP-GEM 商店钻石; 1. SHOP-BUNDLE 商店BUNDLE; 2. 破冰礼包; 3. 每日礼包; 4. 钻石保险箱-小猪; 5. 闪购任务助手; 6. 资源礼包; 7. RVSHOP礼包; 8. 礼包链; 9. 海豹建筑礼包; 10.复活节鸡蛋篮子; 11.复活节积分礼包; 12.鱼塘助手; 13.VIP PASS; 14. 活动续期; 15.体力礼包; 16.气球礼包; 17.神秘礼物礼包; 19.VIP PASS 2期; ; 30.情人节HG礼包; 36. TM礼包链; 37.合成蓝莓礼包; 38.分离每日礼包; 40.西瓜礼包; 42.TMBP; 43.越买越划算礼包; 44.买一赠一礼包; 45.升级礼包; 46.主题装修活动续期; 47.周卡; 101.体力; 48.养狗牛排礼包; 50.进步礼包; 51.自选礼包; 52.郁金香礼包; 53.俩礼包; 55.调制礼包; 56.乌龟对对碰礼包; 57.祖玛礼包; 58.养狗三合一; 59.卡皮巴拉; 60.钉子; 61.圣诞盲盒礼包; 62.卡皮钉子; 63.新破冰; 65.飞镖
    public int area ;
    
    // 是否在商城上架
    public bool onSale ;
    
    // 商城首页显示
    public bool onMainPage ;
    
    // 0-普通; 1-打折促销; 2-最受欢迎; 3-最高性价比
    public int best_deal ;
    
    // 0-钻石; 1-BUNDLE; 2-破冰; 21-低价破冰; 3-每日; 4-PIGGYBANK; 5-闪购任务助手; 6-资源礼包; 7. RVSHOP礼包; 8. 礼包链; 9. 海豹建筑礼包; 10.复活节鸡蛋篮子; 11.复活节积分礼包; 12.鱼塘助手; 13. VIP PASS; 14. 活动续期; 16.体力礼包; 17.气球礼包; 18.神秘礼物礼包; 19.BP 二期; 33.TM复活礼包; 34.TM破冰礼包; 35.TM去插屏礼包; 36. TM礼包链; 37.合成蓝莓礼包; 38.分离每日礼包; 39.三合一礼包; 40.西瓜礼包; 41.新三合一礼包; 42.TMBP; 43.越买越划算礼包; 44.买一赠一礼包; 45.升级礼包; 46.主题装修活动续期; 47.周卡; 48.养狗牛排礼包; 50.进步礼包; 51.自选礼包; 52.郁金香礼包; 53.俩礼包; 55.调制礼包; 56.乌龟对对碰礼包; 57.祖玛礼包; 58.养狗三合一; 59.卡皮巴拉; 60.钉子; 61.圣诞盲盒礼包; 62.卡皮钉子; 63.新破冰; 64.卡皮TILE; 65.飞镖; 71.爬塔
    public int productType ;
    
    // 每日限购次数
    public int lmtNum ;
    
    // 折扣显示：转换成百分数显示
    public float showDiscount ;
    
    // 0-普通商品; 1-不可消耗商品; 2-订阅
    public int purchaseType ;
    
    // 显示的图标
    public string icon ;
    


    public override int GetID()
    {
        return id;
    }
}
