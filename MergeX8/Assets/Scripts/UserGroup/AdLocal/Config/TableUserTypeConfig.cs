using System;
using System.Collections.Generic;

[System.Serializable]
public class UserTypeConfig : TableBase
{   
    // #
    public int Id { get; set; }// 用户分层ID
    public int UserTypeId { get; set; }// 用户分层次序：用于特殊逻辑对跳层进行正负修正
    public int UserTypeSequence { get; set; }// 同参数的分层在取付费区间和转入付费区间对应分层时，只读取与自己LOOPID参数一致的配置
    public int GroupId { get; set; }// 用户分层标签
    public string UserTypeName { get; set; }// 付费用户分层 购买力区间; ; -1 走免费的转化逻辑; -2 走付费沉默层的特殊转化逻辑; 数组 走付费用户查询购买力转化，≥数组1，≤数组2; ; 实际金额数值为配置的整数÷100
    public List<int> PayPowerInterval { get; set; }// 日均购买力统计周期：; ; 计算数组内三个参数所圈定的自然日周期内，玩家的付费总额÷付费天数; ; 所有的分层用户 支付成功 触发转移层级，参考 数组1 和数组2  统计结果的MAX值 转入对应购买力区间分层; ; PAYUSER分层  非付费转化计数＜1时， 参考 数组3的统计结果 转入对应购买力区间分层
    public List<int> PayPowerCycle { get; set; }// 转出类型：; 1、归因分层转出; 2、PAY分层转出; 3、SLLENCE分层转出; 4、FREE分层转出; -1、没有转出逻辑
    public int UserExportType { get; set; }// 用户行为转出触发条件1（满足1中即可继续向下判断）：; 1、用户距离转入该分层日期≥N个活跃天(当天不算); 2、用户累计完成N个任务; 3、用户累计关闭N次SHOP界面; 4、用户连续SKIP N个激励视频场景; 5、用户累计消耗N点体力; ; ; ID类型1 判断 12345; ID类型2 判断 125; ID类型3 判断 15; ID类型4 判断 15
    public List<int> UserExportCondition { get; set; }// 付费用户降档一次之后行为转出触发条件（满足1中即可继续向下判断）：; 1、用户距离转入该分层日期≥N个活跃天(当天不算); 2、用户累计完成N个任务; 3、用户累计关闭N次SHOP界面; 4、用户连续SKIP N个激励视频场景; 5、用户累计消耗N点体力; ; ; ID类型1 判断 12345; ID类型2 判断 125; ID类型3 判断 15; ID类型4 判断 15
    public List<int> PayUserFreeExport { get; set; }// PAYUSER向SILENCE分层转化次数特殊判断：; PAYUSER分层的用户，每次触发免费转化判断计数+1,用户若触发付费转化则重置该计数。; 该计数＜1，PAYUSER用户应按照USEREXPORTCONDITION的条件，转化至30天周期日均购买力对应PAYUSER分组。该计数≥1，则按照PAYUSERFREEEXPORT的条件进行判断，成功后并转入SILENCEUSERRV 302分组。; ; 参数为1，表示该分组开启该逻辑。; 参数为-1，表示该分组不开启该逻辑。
    public int PayUserFreeExportTimes { get; set; }// 高频付费条件：; ; 付费分层用户 付费后触发转化 或者 不付费触发PAYUSERFREEEXPORTTIMES计数=0时追加判断修正：; ; 玩家近5日（包含今日）的付费天数是否≥3天
    public List<int> FrequentlyPayCondition { get; set; }// 窗口期观看激励视频次数MAX值：; ; 1、≤数组1; 2、＞数组1且≤数组2; 3、＞数组2
    public List<int> WatchRvNum { get; set; }// 统计单日激励视频播放次数最高值的周期：按活跃天数统计
    public int WatchRvMaxCycle { get; set; }// 窗口期5S插屏入口播放次数MAX值：; ; 1、≤数组1; 2、＞数组1且≤数组2; 3、＞数组2
    public List<int> Watch5sInterNum { get; set; }// 统计单日5S插屏入口次数最高值的周期：按活跃天数统计
    public int Watch5sInterMaxCycle { get; set; }// 非付费玩家转出判断的目标分层; ; 数组1：WATCHRVNUM  符合3的情况，转入数组1; 数组2：WATCHRVNUM  符合2的情况，转入数组2; 数组3：WATCHRVNUM  符合1的情况，且WATCH5SINTERNUM符合 3的情况，转入数组3; 数组4：WATCHRVNUM  符合1的情况，且WATCH5SINTERNUM符合 1或2的情况，转入数组4; ; ; 
    public List<int> FreeExportTarger { get; set; }// 付费用户转入待付费分层
    public int PayUserWaitPayExport { get; set; }// 付费用户第二次降级转出指定分层
    public int PayUserSecondExport { get; set; }// 保持现有分层：采用现分层参数
    public int HoldNowUsertype { get; set; }// 升级目标分层：采用目标层参数
    public int UpTargetUsertype { get; set; }// 老版本付费用户行为转出触发条件（满足1中即可继续向下判断）; 【注意】老版本付费用户的非付费转出逻辑与新版本用户不一致，老版本付费用户的转出逻辑是一旦满足转出条件，向下一级购买力分层转化，直到转化至305分层后，再次满足非付费转化条件后，转移至302分层后，变为正常转化逻辑。 老用户付费转化走正常逻辑。; ：; 1、用户距离转入该分层日期≥N个活跃天(当天不算); 2、用户累计完成N个任务; 3、用户累计关闭N次SHOP界面; 4、用户连续SKIP N个激励视频场景; 5、用户累计消耗N点体力; ; ; ID类型1 判断 12345; ID类型2 判断 125; ID类型3 判断 15; ID类型4 判断 15
    public List<int> OldUserExport { get; set; }// 老版本付费用户满足OLDUSEREXPORT后，转出的目标分层ID，如本参数为-1则该用户从本次转出开始按照正常付费用户逻辑升降。
    public int OldUserInto { get; set; }// 沉默付费用户，付费后转出目标分层。配置为-1时不开启该逻辑，配置为分层ID数值时，对应分层用户付费后直接转入对应分层
    public int SilenceUserUpInto { get; set; }// 沉默付费用户，不付费转出目标分层。配置为-1时不开启该逻辑，配置为分层ID数值时，对应分层用户触发非付费转化后直接进入参数指向的分层
    public int SilenceUserExportInto { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
