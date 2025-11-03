using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Newtonsoft.Json.Linq;
using UnityEngine;


public class MasterCardModel : Singleton<MasterCardModel>
{
    private StorageMasterCard _masterCard;

    public bool isOpen = false;

    /// <summary>
    /// "奖励类型
    // 1 去插屏
    // 2 充值钻石翻倍
    // 3 增加背包格数
    // 4 建筑cd减少
    // 5 体力上限增加
    // 101 金币
    // 102 钻石
    // 201 体力"
    /// </summary>
    public enum MCRewardType
    {
        NoAds = 1,
        PayDouble = 2,
        AddBag = 3,
        ReduceCD = 4,
        MaxEnergy = 5,
        AddProduct = 6,
        Coin = 7,
        Diamond = 8,
        Energy = 9,
    }

    public StorageMasterCard MasterCard
    {
        get
        {
            if (_masterCard == null)
                _masterCard = StorageManager.Instance.GetStorage<StorageHome>().MasterCard;

            return _masterCard;
        }
    }

    public bool IsBuyMasterCard
    {
        get { return MasterCard.EndTime - (long) APIManager.Instance.GetServerTime() > 0; }
    }


    public int LeftRewardCount
    {
        get { return MasterCard.LeftRewardCount; }
    }

    public bool IsMasterCardResource(int resourceId)
    {
        foreach (MCRewardType item in Enum.GetValues(typeof(MCRewardType)))
        {
            if ((int) item == resourceId)
                return true;
        }

        return false;
    }

    public void InitOpenToggle()
    {
        CGetConfig cGetConfig = new CGetConfig
        {
            Route = "MasterCard_" + AssetConfigController.Instance.RootVersion,
        };

        APIManager.Instance.Send(cGetConfig, (SGetConfig sGetConfig) =>
        {
            if (string.IsNullOrEmpty(sGetConfig.Config.Json))
            {
                DebugUtil.LogWarning("MasterCard 服务器配置为空！");
                return;
            }

            JObject obj = JObject.Parse(sGetConfig.Config.Json);

#if UNITY_ANDROID
            isOpen = int.Parse(obj["Android"].ToString()) == 1;
#elif UNITY_IOS
         isOpen = int.Parse(obj["iOS"].ToString()) == 1;
#else
         isOpen = int.Parse(obj["Android"].ToString()) == 1;
#endif
        }, (errno, errmsg, resp) => { });
    }

    public int MasterCardId()
    {
        return MasterCard.MasterCardId;
    }

    public string GetMasterCareLeftTime()
    {
        if (!IsBuyMasterCard)
        {
            return LocalizationManager.Instance.GetLocalizedString("UI_master_card_title");
        }

        return CommonUtils.FormatLongToTimeStr((long) MasterCard.EndTime - (long) APIManager.Instance.GetServerTime(),
            false);
    }

    public int GetPayDouble()
    {
        return MasterCard.LeftPayDoubleCount;
    }

    public void SetPayDouble(int count)
    {
        MasterCard.LeftPayDoubleCount = count;
    }

    public bool IsOpen()
    {
        return isOpen && MasterCardId() > 0 && UnlockManager.IsOpen(UnlockManager.MergeUnlockType.MasterCard);
    }

    public void InitMasterCard()
    {
        if (MasterCardId() <= 0)
        {
            InitMasterCardData();
        }

        if ((long) APIManager.Instance.GetServerTime() > MasterCard.EndTime)
        {
            InitMasterCardData();
        }
    }

    private void InitMasterCardData()
    {
        MasterCard.MasterCardId = AdConfigHandle.Instance.GetMasterCardId();
    }

    public MasterCardList GetMasterCardList()
    {
        int id = MasterCardId();

        if (id <= 0)
            return null;

        return AdConfigHandle.Instance.GetMasterCardList(id);
    }

    public List<MasterCardResource> GetMasterCardDatas()
    {
        int id = MasterCardId();

        if (id <= 0)
            return null;

        return AdConfigHandle.Instance.GetMasterCardDatas(id);
    }

    public MasterCardResource GetMasterCardResource(MCRewardType rewardType)
    {
        List<MasterCardResource> listData = GetMasterCardDatas();
        if (listData == null || listData.Count == 0)
            return null;

        foreach (var data in listData)
        {
            if (data.Type == (int) rewardType)
                return data;
        }

        return null;
    }

    private int GetAddBagNumByAdConfig()
    {
        List<MasterCardResource> listData = GetMasterCardDatas();
        if (listData == null || listData.Count == 0)
            return 0;

        foreach (var data in listData)
        {
            if (data.Type != 3)
                continue;

            return data.RewardParam;
        }

        return 0;
    }

    public int GetAddBagNum()
    {
        if (!IsBuyMasterCard)
            return 0;
        return MasterCard.AddBagNum;
    }

    public void PurchaseSuccess(TableShop config)
    {
        if (config == null)
            return;

        MasterCardList masterCardList = GetMasterCardList();
        if (masterCardList == null)
            return;

        int day = 0;
        if (masterCardList.Buy7_shopId == config.id)
        {
            day = 7;
        }
        else if (masterCardList.Buy30_shopId == config.id)
        {
            day = 30;
        }

        if ((long) APIManager.Instance.GetServerTime() > MasterCard.EndTime)
        {
            Debug.LogWarning("MasterCard 重新购买 " + day);
        }
        else
        {
            Debug.LogWarning("MasterCard 续费 " + day);
        }

        AddMasterCardDay(day);
        var payDouble = GetMasterCardResource(MCRewardType.PayDouble);
        MasterCard.LeftPayDoubleCount += payDouble == null ? 0 : payDouble.RewardParam;
        MasterCard.IsRecharge7 = day == 7;
        MasterCard.IsRecharge30 = day == 30;
        MasterCard.LeftRewardCount += day;
        MasterCard.GetRewardTime = (long) APIManager.Instance.GetServerTime();
        MasterCard.AddBagNum = GetAddBagNumByAdConfig();
        MasterCard.IsPopupRenewal = false;

        PopUpMasterCardPurchaseReward(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap, "purchase", config.id.ToString());

        EventDispatcher.Instance.DispatchEvent(EventEnum.MASTERCARD_PURCHASE, config);
    }

    public void PopUpMasterCardPurchaseReward(BiEventAdventureIslandMerge.Types.ItemChangeReason reason, string data1, string data2,
        Action endCall = null)
    {
        List<MasterCardResource> listData = GetMasterCardDatas();
        if (listData == null || listData.Count == 0)
        {
            endCall?.Invoke();
            return;
        }

        _masterCard.LeftRewardCount -= 1;
        if (_masterCard.LeftRewardCount < 0)
            _masterCard.LeftRewardCount = 0;
        var ret = new List<ResData>();
        foreach (var data in listData)
        {
            if (!UserData.Instance.IsResource(data.Type))
            {
                ret.Add(new ResData(data.Type, data.RewardParam));
                continue;
            }

            ret.Add(new ResData(data.Type, data.RewardParam));
        }

        MasterCard.GetRewardTime = (long) APIManager.Instance.GetServerTime();

        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(reason);
        reasonArgs.data1 = data1;
        reasonArgs.data2 = data2;
        EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);

        CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,
            reasonArgs, () =>
            {
                endCall?.Invoke();
                EventDispatcher.Instance.DispatchEvent(EventEnum.MASTERCARD_GETREWARD);
            });
    }

    public void PopUpMasterCardReward(BiEventAdventureIslandMerge.Types.ItemChangeReason reason, string data1, string data2,
        Action endCall = null)
    {
        List<MasterCardResource> listData = GetMasterCardDatas();
        if (listData == null || listData.Count == 0)
        {
            endCall?.Invoke();
            return;
        }

        int canGetDays = 1;
        if (_masterCard.GetRewardTime > 0)
        {
            canGetDays = Utils.GetDayInterval(MasterCard.GetRewardTime / 1000,
                (long) APIManager.Instance.GetServerTime() / 1000);
            if (canGetDays > _masterCard.LeftRewardCount)
                canGetDays = _masterCard.LeftRewardCount;
        }

        canGetDays = Mathf.Max(1, canGetDays);

        _masterCard.LeftRewardCount -= canGetDays;
        if (_masterCard.LeftRewardCount < 0)
            _masterCard.LeftRewardCount = 0;
        var ret = new List<ResData>();
        foreach (var data in listData)
        {
            if (!UserData.Instance.IsResource(data.Type))
                continue;

            ret.Add(new ResData(data.Type, data.RewardParam * canGetDays));
        }

        MasterCard.GetRewardTime = (long) APIManager.Instance.GetServerTime();

        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(reason);
        reasonArgs.data1 = data1;
        reasonArgs.data2 = data2;
        CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,
            reasonArgs, () =>
            {
                endCall?.Invoke();
                EventDispatcher.Instance.DispatchEvent(EventEnum.MASTERCARD_GETREWARD);
            });
    }

    private void AddMasterCardDay(int day)
    {
        long addTime = (long) day * (24 * 3600 * 1000);
        if ((ulong) MasterCard.EndTime < APIManager.Instance.GetServerTime())
        {
            MasterCard.EndTime = (long) APIManager.Instance.GetServerTime() + addTime;
        }
        else
        {
            MasterCard.EndTime += addTime;
        }
    }

    public int GetMaxEnergy()
    {
        var maxEnergy = GetMasterCardResource(MCRewardType.MaxEnergy);
        if (maxEnergy == null)
            return 100;

        return maxEnergy.RewardParam;
    }

    /// <summary>
    /// 获取减少CD百分比
    /// </summary>
    /// <returns></returns>
    public int GetReduceCDPre()
    {
        var mcRes = GetMasterCardResource(MCRewardType.MaxEnergy);
        if (mcRes == null)
            return 0;
        return mcRes.RewardParam;
    }

    public int GetProductAddPre()
    {
        var mcRes = GetMasterCardResource(MCRewardType.AddProduct);
        if (mcRes == null)
            return 0;
        return mcRes.RewardParam;
    }

    /// <summary>
    /// 钻石购买额外百分比
    /// </summary>
    /// <returns></returns>
    public int GetPayDoublePre()
    {
        var mcRes = GetMasterCardResource(MCRewardType.PayDouble);
        if (mcRes == null)
            return 0;
        return mcRes.ExtraParam;
    }

    public bool IsCanPopupRenewal()
    {
        if ((long) APIManager.Instance.GetServerTime() <= MasterCard.EndTime)
            return false;

        if (!MasterCard.IsRecharge7 && !MasterCard.IsRecharge30)
            return false;

        return !MasterCard.IsPopupRenewal;
    }
}