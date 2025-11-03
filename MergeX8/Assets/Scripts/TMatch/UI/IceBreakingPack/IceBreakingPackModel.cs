using System;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.Config.TMatch;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Framework;
using MiniGame;

namespace TMatch
{
    public class IceBreakingPackModel : GlobalSystem<IceBreakingPackModel>, IInitable, IUpdatable
{
    private StorageIceBreakingPack _storage;
    public int showPackId;
    // 当前游戏状态，0为默认表示未进行闯关，1为开始闯关，2为闯关成功
    private int nowGameStatus = 0;
    private int winStreakTimes = 0;
    public void Init()
    {
        _storage = StorageManager.Instance.GetStorage<StorageTMatch>().IceBreakingPack;
        // init调用目的：防止第二天未及时刷新在线时长，导致触发的礼包不正确
        TryUpdateDayOnlineTime(1);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_WIN, OnGameWin);
        EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_START, onGameStart);
        EventDispatcher.Instance.AddEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
    }

    public void Release()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_WIN, OnGameWin);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_START, onGameStart);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
    }

    public bool IsEnterSuccess = false;
    private float countTime = 0;
    public void Update(float deltaTime)
    {
        if (!IsEnterSuccess) return;
        countTime += deltaTime;
        if (countTime > 10)
        {
            countTime -= 10;
            TryUpdateDayOnlineTime(10);
        }
    }

    /// <summary>
    /// 更新在线时长
    /// </summary>
    public void TryUpdateDayOnlineTime(int addSeconds)
    {
        var nowTime = CommonUtils.GetTimeStamp();
        var zeroTime = GetZeroTimeStamp();
        if (_storage.LastOnlineTime <= zeroTime)
        {
            _storage.OnlineTime = 0;
        }
        _storage.LastOnlineTime = nowTime;
        _storage.OnlineTime += addSeconds;
    }

    /// <summary>
    /// 游戏胜利立即更新记录今天的胜利次数
    /// </summary>
    /// <param name="evt"></param>
    public void OnGameWin(BaseEvent evt)
    {
        _storage.WinStreakTimes = winStreakTimes + 1;
        nowGameStatus = 2;
    }

    /// <summary>
    /// 进入游戏记录
    /// </summary>
    /// <param name="evt"></param>
    public void onGameStart(BaseEvent evt)
    {
        nowGameStatus = 1;
        winStreakTimes = _storage.WinStreakTimes;
        _storage.WinStreakTimes = 0;
    }

    /// <summary>
    /// 尝试弹出破冰礼包弹窗
    /// </summary>
    /// <returns></returns>
    public bool TryToShowPopup()
    {
        if (_storage.IsBuy)
            return false;
        
        // 刚进入大厅不弹
        // if (nowGameStatus == 0) return false;
        // 分组里面的条件判断
        // var userGroup = IceBreakingPackConfig.Model.Instance.Group;
        // if (userGroup == null || !userGroup.IsOpen) return false;
        // // 等级判断
        int unLockLevel  = GlobalConfigManager.Instance.GetNumValue("TM_icebreak_unlock");
        if (unLockLevel > TMatchModel.Instance.GetMainLevel())
            return false;
   
        // // 玩家付过费的条件判断
        // if (IAPController.Instance.model.IsPlayerPurchased() && !userGroup.PaidShow) return false;
        // // 注册时间判断
        // var installAt = (long)StorageManager.Instance.GetStorage<StorageCommon>().InstalledAt;
        var nowTime = CommonUtils.GetTimeStamp();
        // var totalTime = nowTime - installAt;
        // if (userGroup.MaxRegisterTime * 24 * 60 * 60 * 1000 < totalTime) return false;
        
        // 闯关完成返回大厅时
        // if (MyMain.myGame.Fsm.PreviousState.Type == FsmStateType.TripleMatch)
        // {
        //     // 如果成功，并且成功次数小于配置的次数时，不弹
        //     if (nowGameStatus == 2 && _storage.WinStreakTimes < userGroup.MinWinTimes) return false;
        // }
        // // 从其它地方进入大厅（包括登录，asmr返回大厅，切换多语言触发的大厅检查链）
        // else
        // {
        //连赢大于3立马就弹
        if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.TripleMatch)
        {
            if (nowGameStatus == 2 && _storage.WinStreakTimes < 3)
            {
                return false;
            }
        }
        else
        {
            if (nowTime - _storage.LastShowTime <300 * 1000) return false; // 这里只判断冷却时间是否到了
        }
        // }
        
        Common common = AdConfigHandle.Instance.GetCommon();
        
        // 没有对应礼包链
        if (common.TMIceBreakPack <= 0) return false;
        
        var isShow = false;
        void CheckShowPack(int checkChainIdx, bool checkStartNew = false, long lastEndTime = 0)
        {
            var chainId = common.TMIceBreakPack;
            var chain = GetChainByGroup(chainId);
            if (chain == null)
            {
                DebugUtil.LogError($"没有找到破冰礼包链配置[id:({chainId})]");
                return;
            }

            if (checkStartNew && nowTime < lastEndTime + chain.Cd * 1000) return;

            int packIdx = 0;
            if (GetPackShowTimes(chain.ChainPacks[0]) > 0)
                packIdx = 1;
            //筛出显示次数匹配的礼包
            // var packIdx = chain.ChainPacks.FindLastIndex(pid =>
            // {
            //     var packConfig = GetIceBreakingPackConfig(pid);
            //     return  GetPackShowTimes(pid)< packConfig.ShowLimit;
            // });

            if (packIdx < 0) return;
            
            var pid = chain.ChainPacks[packIdx];
            var packCfg = GetIceBreakingPackConfig(pid);
            var buyTimes = GetPackBuyTimes(pid);
            var showTimes = GetPackShowTimes(pid);
            if (showTimes >= packCfg.ShowDayLimit || buyTimes >= packCfg.BuyLimit) return;  //展示和购买条件不符合

            isShow = true;
            showPackId = chain.ChainPacks[packIdx];
            
            _storage.ChainId = chainId;//debug等方式可能会修改chainId，并不是只有开启新链才赋值
            
            if (!checkStartNew) return;
            //开启一轮
            _storage.ChainIndex = checkChainIdx;
            _storage.ChainStartTime = nowTime;
            _storage.ChainEndTime = nowTime + chain.Duration * 1000;
            _storage.PackBuyTimes.Clear();
            _storage.PackShowTimes.Clear();
        }
        
        if (_storage.ChainEndTime == 0)
        {
            //没有历史记录，尝试直接开启当前idx的
            CheckShowPack(_storage.ChainIndex, true);
        }
        else if (_storage.ChainEndTime > nowTime)
        {
            //在历史记录时间范围内，数据有效
            CheckShowPack(_storage.ChainIndex);
        }
        else
        {
            // //超过历史记录时间，需要检测是否能开启下一个礼包链
            var nextIdx = _storage.ChainIndex ;
            CheckShowPack(nextIdx, true, _storage.ChainEndTime);
        }

        if (isShow)
        {
            UIViewSystem.Instance.Open<IceBreakingPackPopup>();
        }
        
        return isShow;
    }

    public TMIceBreakPackChain GetChainByGroup(int groupId)
    {
        return AdConfigExtendConfigManager.Instance.TMIceBreakPackChainList.Find(a => a.Groupid == groupId);
    }

    public TMIceBreakPack GetIceBreakingPackConfig(int id)
    {
        return AdConfigExtendConfigManager.Instance.TMIceBreakPackList.Find(a => a.Id == id);

    }
    public TMIceBreakPack GetIceBreakingPackConfigByShopId(int id)
    {
        return AdConfigExtendConfigManager.Instance.TMIceBreakPackList.Find(a => a.ShopId == id);

    }
    /// <summary>
    /// 获取礼包的购买次数
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetPackBuyTimes(int id)
    {
        if (_storage.PackBuyTimes.TryGetValue(id, out int times))
        {
            return times;
        }

        return 0;
    }

    /// <summary>
    /// 获取礼包的当天展示次数
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetPackShowTimes(int id)
    {
        var nowTime = CommonUtils.GetTimeStamp();
        var zeroTime = GetZeroTimeStamp();
        // 不是同一天，则直接清理掉记录
        if (_storage.LastShowTime <= zeroTime)
        {
            _storage.PackShowTimes.Clear();
        }
        
        if (_storage.PackShowTimes.TryGetValue(id, out int times))
        {
            return times;
        }

        return 0;
    }
    
    /// <summary>
    /// 获取当前展示的礼包的配置切将弹窗弹出次数记录+1(此方法仅限弹窗弹出时调用)
    /// </summary>
    /// <returns></returns>
    public TMIceBreakPack GetShowPackConfigAndAddRecord()
    {
        if (_storage.PackShowTimes.TryGetValue(showPackId, out int times))
        {
            _storage.PackShowTimes[showPackId] = times + 1;
        }
        else
        {
            _storage.PackShowTimes.Add(showPackId, 1);
        }

        _storage.PopTotalTimes++;
        _storage.LastShowTime = CommonUtils.GetTimeStamp();
        _storage.WinStreakTimes = 0;
        return GetIceBreakingPackConfig(showPackId);
    }

    public void BuySuccess(int shopId)
    {
        var packId = GetIceBreakingPackConfigByShopId(shopId).Id;
        if (_storage.PackBuyTimes.TryGetValue(packId, out int times))
        {
            _storage.PackBuyTimes[packId] = times + 1;
        }
        else
        {
            _storage.PackBuyTimes.Add(packId, 1);
        }
    }
    
    /// <summary>
    /// 补单处理
    /// </summary>
    /// <param name="evt"></param>
    private void OnIAPSuccessEvent(BaseEvent evt)
    {
        IAPSuccessEvent drivedEvt = evt as IAPSuccessEvent;
        if (drivedEvt.shop != null )//&& (drivedEvt.userData as Type) == typeof(UnfulfilledPaymentsNoticeController)
        {
            var shopConfig = TMatchConfigManager.Instance.ShopConfigList.Find(item => item.id == drivedEvt.shop.id);

            if (shopConfig.shopType == (int)IAPShopType.IceBreaking)
            {
                BuySuccess(drivedEvt.shop.id);
            }
        }
        
        _storage.ChainEndTime = CommonUtils.GetTimeStamp();//通过修改链结束时间完成重置
        _storage.IsBuy = true;
    }

    /// <summary>
    /// 这才是获取本地时间0点时间戳的正确方法，CommonUtils.GetZeroTimeStamp 获取到的是UTC 0点
    /// </summary>
    /// <returns></returns>
    private long GetZeroTimeStamp()
    {
        // 获取当前本地时间
        DateTime now = DateTime.Now;
        // 获取本地时区的零点时间
        DateTime zeroTime = now.Date;
        // 将本地时区的零点时间转换为 UTC 时间
        DateTime utcZeroTime = zeroTime.ToUniversalTime();
        // 获取当天零点的毫秒值
        long milliseconds = (long)(utcZeroTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        return milliseconds;
    }
    
}
}
