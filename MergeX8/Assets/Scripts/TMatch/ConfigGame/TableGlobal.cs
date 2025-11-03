/************************************************
 * Config class : Global
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;

namespace DragonPlus.Config.TMatchShop
{
    [System.Serializable]
    public class Global
    {   
        
        // 体力---最大能量值
        public int MaxUserEnergy ;
        
        // 能量值花费的钻石
        public int EnergyGemPrice ;
        
        // 体力---自动回复能量的时间间隔(秒)
        public int EnergyRefillTime ;
        
        // 体力---每次回复多少能量
        public int EnergyRefillAmount ;
        
        // FACEBOOK 登录功能开放的关卡限制(通过此关)
        public int FacebookLoginUnlockLevel ;
        
        // FB好友赠送体力每日最大领取数量
        public int FBFriendEnergyDailyClaimLimitation ;
        
        // 好友邀请奖励
        public int InviteRewardDiamond ;
        
        // 好友邀请奖励的关卡限制
        public int InviteRewardLevel ;
        
        // 活动拉取CD（秒）
        public int SeverActivityDataFetchCD ;
        
        // 钻石金币兑换比（1钻石兑换多少金币）
        public int diamondToGold ;
        
        // 名字长度限制
        public int NameLength ;
        
        // 改名字CD时间（小时）
        public int nameChangeCd ;
        
        // 战令礼包金币
        public int battlePassTip ;
        
    }
}
