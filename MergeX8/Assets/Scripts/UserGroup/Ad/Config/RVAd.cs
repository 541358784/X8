// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : RVAd
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.ConfigHub.Ad
{
    public class RVAd
    {   
        // #
        public int Id { get; set; }// 渠道、国家; 11-IOS美国; 12-IOS非美国; 21-安卓美国; 22-安卓非美国
        public List<int> PlatformGroup { get; set; }// 对应COMMON的ID; 101. 首次进入游戏; 102. 新增3日内未付费; 201. 96小时未付费; 301. 首次付费大于24小时小于等于48; 302. 首次付费大于48小时小于等于72; 303. 首次付费大于72小时小于等于96; 304. 首次付费大于96小时; 401. 首次付费小于24小时
        public List<int> SubUserGroup { get; set; }// 用户分层ID
        public List<int> UserTypeId { get; set; }// SERVER用户组
        public List<int> UserGroup { get; set; }// 单次最大付费金额; （用区间表示，左闭右开）
        public List<int> MaxpayGroup { get; set; }// 活跃度; 101-活跃时长高、完成任务高; 102-活跃时长高、完成任务低; 103-活跃时长低、完成任务高; 104-活跃时长低、完成任务低
        public List<int> ActiveGroup { get; set; }// 显示位置
        public string PlaceId { get; set; }// 是否播放该点位展示RV; 0. 不播放; 1. 播放
        public int IfOpen { get; set; }// 间隔时间(秒; COMMONMONETIZATIONEVENTREASONADSEPERATECOOLDOWN
        public int ShowInterval { get; set; }// 每日次数限制; COMMON_MONETIZATION_EVENT_REASON_AD_OVERDISPLAY
        public int LimitPerDay { get; set; }// 奖励ID，见BONUS表
        public int Bonus { get; set; }// 跳过CD时长
        public int SkipCD { get; set; }// 每日限制还要参考哪些其他的配置（请填PLACEID)
        public List<int> LimitPerDayCoeffect { get; set; }// 需要看广告次数
        public int WatchTimes { get; set; }// 广告递增系数
        public List<int> Price_idex { get; set; }// 关闭广告入口计数; ; 0 不计数; 1 计数+1
        public int SkipShowCount { get; set; }// 广告入口是否可替换为5S插屏入口; ; 0 不可以; ; 1 可以
        public int ActiveInterAdsChange { get; set; }// 入口图标动画播放间隔（秒）
        public int IconSpineCD { get; set; }// 替换为5S插屏时使用的插屏广告ID
        public string ActiveInterAdsPlaceId { get; set; }
    }
}