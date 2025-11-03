// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * ConfigHub class : InterstitialAd
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System.Collections.Generic;

namespace DragonPlus.ConfigHub.Ad
{
    public class InterstitialAd
    {   
        // #
        public int Id { get; set; }// 渠道、国家; 11-IOS美国; 12-IOS非美国; 21-安卓美国; 22-安卓非美国
        public List<int> PlatformGroup { get; set; }// 对应COMMON的ID; 101. 首次进入游戏; 102. 新增3日内未付费; 201. 96小时未付费; 301. 首次付费大于24小时小于等于48; 302. 首次付费大于48小时小于等于72; 303. 首次付费大于72小时小于等于96; 304. 首次付费大于96小时; 401. 首次付费小于24小时
        public List<int> SubUserGroup { get; set; }// SERVER用户组
        public List<int> UserGroup { get; set; }// 用户分层ID
        public List<int> UserTypeId { get; set; }// 单次最大付费金额; （用区间表示，左闭右开，-1表示正无穷）
        public List<int> MaxpayGroup { get; set; }// 活跃度; 10001-活跃时长高、完成任务高; 10002-活跃时长高、完成任务低; 10003-活跃时长低、完成任务高; 10004-活跃时长低、完成任务低
        public List<int> ActiveGroup { get; set; }// 显示位置
        public string PlaceId { get; set; }// 是否播放该点位的插屏; 0. 不播放; 1. 播放
        public int IfOpen { get; set; }// 间隔秒
        public int Interval { get; set; }// 次数 每天上限
        public int LimitPerDay { get; set; }// 开启等级; >0用此等级控制; 否则用COMMON里的控制
        public int UnlockLevel { get; set; }// 需要看广告次数
        public int WatchTimes { get; set; }// 是否为主动插屏入口ID; 0 不是; 1 是
        public int IsActiveInterAds { get; set; }
    }
}