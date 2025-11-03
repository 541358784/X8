/************************************************
 * Config class : TableMergeItem
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TableMergeItem : TableBase
{   
    
    // ID
    public int id ;
    
    // 物品名称多语言ID
    public string name_key ;
    
    // 图片路径
    public string image ;
    
    // 可生产时图片
    public string image_full ;
    
    // 任务图标偏移
    public float offsetY ;
    
    // 是否可以堆叠
    public bool canStacking ;
    
    // 初始堆叠数量
    public int defaultStackNum ;
    
    // 类型
    public int type ;
    
    // 子类型; 1. 套娃
    public int subType ;
    
    // 忽略无CD 道具
    public bool ignoreCdProp ;
    
    // 是否显示文本提示
    public bool showText ;
    
    // 动画提示资源名
    public string showAnimAsset ;
    
    // 等级
    public int level ;
    
    // 所在合成链
    public int in_line ;
    
    // 来源
    public int[] re_line ;
    
    // 物品特殊来源
    public int[] reSp_line ;
    
    // 是否订单任务目标
    public bool order_possible ;
    
    // 多语言说明
    public string item_des ;
    
    // 上个等级
    public int pre_level ;
    
    // 下个等级
    public int next_level ;
    
    // 出售价格(金币)
    public int sold_gold ;
    
    // 是否蛛网产出
    public int output_lock ;
    
    // 是否受能量狂潮影响
    public bool energy_frenzy ;
    
    // 是否全部产出后进入CD
    public bool is_product_all_cd ;
    
    // 每轮CD时间（分钟）
    public float cd_time ;
    
    // 产出限制目前的用户等级
    public int[] outputLimit ;
    
    // 产出限制连续次数
    public int[] outputLimitByTimes ;
    
    // 动态权重-道具
    public int[] dynamicPowerItem ;
    
    // 动态权重
    public int dynamicPower ;
    
    // 激活消耗
    public int[] active_cost ;
    
    // 开始产出消耗; -1 动态生成
    public int[] produce_cost ;
    
    // 开始产出是否忽略CD
    public bool isIgnoreCd ;
    
    // CD加速消耗
    public int[] cdspeed_cost ;
    
    // 产出消耗
    public int[] output_cost ;
    
    // 产出对象
    public int[] output ;
    
    // 产出权重
    public int[] power ;
    
    // 每轮产出数量
    public int output_amount ;
    
    // 最大库存数量
    public int max_output_amount ;
    
    // 时间产出CD加速消耗
    public int[] time_cdspeed_cost ;
    
    // 时间产出消耗
    public int[] timeOutput_cost ;
    
    // 时间产出对象
    public int[] timeOutput ;
    
    // 时间产出权重
    public int[] timeOutputPower ;
    
    // 时间产出每轮数量
    public int time_output_amount ;
    
    // 时间产出最大库存数量
    public int time_max_output_amount ;
    
    // 保底次数
    public int[] Max_Drop_Interval ;
    
    // 显示产出权重
    public int[] show_power ;
    
    // 只配合死亡掉落用; 0 不走死亡产出掉落; 配置1 产出1轮 死亡; 配置2 产出2轮 死亡; 。。。
    public int onelife ;
    
    // 死亡掉落
    public int out_death ;
    
    // 货币对应数值
    public int value ;
    
    // 气泡生成概率（%）
    public int[] bubble_rate ;
    
    // 气泡解锁权重
    public int[] unlcok_power_bubble ;
    
    // 气泡解锁的花费(钻石）
    public int unlcok_cost_bubble ;
    
    // 每日出现气泡的最大次数
    public int day_bubble_count ;
    
    // 蛛网解锁的花费（钻石）
    public int unlcok_cost_net ;
    
    // 道具的参数
    public int booster_factor ;
    
    // 可以分解
    public int Gem_split ;
    
    // 是否可以RV加速CD
    public bool rv_speedup ;
    
    // 卖出时二次确认
    public bool sold_confirm ;
    
    // RV加速建筑充能次数（只对时间建筑生效）
    public int rv_speed_count ;
    
    // 建筑产出量初始产出值
    public int original_count ;
    
    // 时间建筑产出量初始产出值
    public int time_original_count ;
    
    // 图鉴奖励，只能有一种货币; 格式：（货币ID，数量）; ; 如果不配就不算到图鉴
    public int[] gallery_award ;
    
    // 物品价值
    public int price ;
    
    // 是否为稀有物品
    public bool isSpecial ;
    
    // 
    public int output_rules_task ;
    
    // MASTERCARD产出不生效
    public bool master_card_undo ;
    
    // 合成产出
    public int merge_output ;
    
    // 合成产出特效 (1~4 有效)
    public int merge_effect_index ;
    
    // 箱子中物品概率显示
    public int[] output_probability ;
    
    // 大小CD
    public int[] big_smale_cd ;
    
    // 大小CD加速消耗
    public string big_smale_cdspeed ;
    
    // 回收转化内容
    public int recovery_item ;
    
    // 红紫箱子产出
    public int[] level_box_produce ;
    
    // 是否放入建筑背包
    public bool isBuildingBag ;
    


    public override int GetID()
    {
        return id;
    }
}
