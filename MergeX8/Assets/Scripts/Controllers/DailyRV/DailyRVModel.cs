using System;
using System.Collections;
using System.Collections.Generic;
using ABTest;
using DragonPlus;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Game;
using Manager;
using UnityEngine;
using static Gameplay.UserData;


public enum ItemState
{
    AlreadyGot,
    CanGet,
    Locked,
    Arrow
}

public class DailyRVModel : Manager<DailyRVModel>
{
    private List<RVshopResource> rVshopResources = new List<RVshopResource>();
    private StorageAdData _storage => StorageManager.Instance.GetStorage<StorageHome>().AdData;

    public int CurRvShopElementIdx
    {
        get { return _storage.CurRVShopListIndex; }
    }

    public List<RVshopResource> GetCurRVShopList()
    {
        if (rVshopResources.Count <= 0)
        {
            RVshopList shopList = AdConfigHandle.Instance.GetRvShopListByID(_storage.CurRVShopListID);
            if (shopList == null)
                shopList = GetRvShopList();

            if (shopList == null)
                return null;

            shopList.List.ForEach(id =>
            {
                RVshopResource rVshopResource = AdConfigHandle.Instance.GetRVshopResource(id);
                rVshopResources.Add(rVshopResource);
            });
        }

        return rVshopResources;
    }

    public bool IsRVShopItemInCurListPurple(int elementIdx)
    {
        return false;
    }

    public bool IsRVShopInCd()
    {
        return _storage.CurRVShopListID > 0;
    }

    public void UpdateRVShopState()
    {
        if (IsRVShopInCd() == false)
            return; // not opened

        System.DateTime dt = CommonUtils.ConvertFromUnixTimestamp((ulong) _storage.RVShopOpenTime);
        System.DateTime dateNow = System.DateTime.Now;
        if (dateNow.Year > dt.Year || dateNow.DayOfYear > dt.DayOfYear)
        {
            RefreshRVShopList();
        }
    }

    public bool IsLastIndexInCurRVShopList(int elementIdx)
    {
        elementIdx += 1;
        while (elementIdx < GetCurRVShopList().Count)
        {
            if (CheckRVShopListReward(elementIdx))
                return false;

            elementIdx++;
        }

        return true;
    }


    public string GetCurRVShopListRestTimeString()
    {
        System.DateTime ot = CommonUtils.ConvertFromUnixTimestamp((ulong) _storage.RVShopOpenTime);
        System.DateTime nd = ot + new System.TimeSpan(24, 0, 0);
        System.DateTime zerod = new DateTime(nd.Year, nd.Month, nd.Day);

        if (System.DateTime.Now >= zerod)
        {
            return ("00m00s");
        }

        System.TimeSpan ds = zerod - System.DateTime.Now;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", ds.Hours, ds.Minutes, ds.Seconds);
    }


    public RvShopPriceRules MatchPriceRules()
    {
        var rules = AdConfigHandle.Instance.GetRvShopPriceRules();
        //昨日是否付费
        int isPayLastDay = 0;
        if (_storage.GotShopItem.Count > 0)
        {
            _storage.UnPayDays = 0;
            isPayLastDay = 1;
            _storage.PayTime++;
        }
        else
        {
            if (_storage.IsInit)
                _storage.UnPayDays++;
        }

        foreach (var rule in rules)
        {
            if (_storage.LastPriceChange != rule.Last_price_change)
                continue;
       
            if (isPayLastDay != rule.Is_pay_last_show)
                continue;
            if (rule.Pay_times > 0 && _storage.PayTime != rule.Pay_times)
                continue;
            if (_storage.UnPayDays != rule.Unpay_show_days)
                continue;

            DebugUtil.Log("------DailyRv-----匹配到 Rules" + rule.Id);
            return rule;
        }

        return null;
    }
    void RefreshRVShopList()
    {
        var rvShopPrice = AdConfigHandle.Instance.GetRvShopPrice();
        if (rvShopPrice == null)
            return;
        var rules = MatchPriceRules();

        //生成价格和折扣----------------------------
        if (!_storage.IsInit)
        {
            _storage.CurrentPrice = rvShopPrice.Start_price;
            _storage.IsInit = true;
        }

        if (rules != null)
        {
            _storage.CurrentPrice = _storage.CurrentPrice + rules.Price_change;
            if (_storage.CurrentPrice < 0)
                _storage.CurrentPrice = 0;
            if (_storage.CurrentPrice >= rvShopPrice.Groupid.Count)
                _storage.CurrentPrice = rvShopPrice.Groupid.Count - 1;
            _storage.UnPayDays = 0;
            _storage.PayTime = 0;
            _storage.LastPriceChange = rules.Price_change;
        }

        if (_storage.CurrentPrice > rvShopPrice.Groupid.Count - 1)
            _storage.CurrentPrice = 0;
        int price = rvShopPrice.Groupid[_storage.CurrentPrice];
        
        _storage.CurRVShopListID = 0;
        _storage.CurRVShopListIndex = 0;
        _storage.RVShopOpenTime = CommonUtils.GetCurTime();
        _storage.CurRVShopGotRecord.Clear();
        _storage.LastRVShopPopupTime = 0;
        rVshopResources.Clear();
        _storage.GotShopItem.Clear();
        RVshopList shopList =GetRvShopList();
        CoolingTimeManager.Instance.RemoveCooling(CoolingTimeManager.CDType.LossTime, ADConstDefine.RV_TV_REWARD);
        EventDispatcher.Instance.DispatchEvent(EventEnum.M3_RV_SHOP_REFRESH);
        if (shopList == null) return;
        _storage.CurRVShopListID = shopList.Id;
    }

    RVshopList GetRvShopList()
    {
        var rvShopPrice = AdConfigHandle.Instance.GetRvShopPrice();
        if (rvShopPrice == null)
            return AdConfigExtendConfigManager.Instance.GetConfig<RVshopList>()[0];;
        
        if (_storage.CurrentPrice > rvShopPrice.Groupid.Count - 1)
            _storage.CurrentPrice = 0;
        int price = rvShopPrice.Groupid[_storage.CurrentPrice];
        List<RVshopList> rvShopLists = AdConfigHandle.Instance.GetRvShopListByGroup(price);
        int level = ExperenceModel.Instance.GetLevel();
        for (int i = 0; i < rvShopLists.Count; i++)
        {
            if (level <= rvShopLists[i].Level)
                return rvShopLists[i];
        }
            
        return rvShopLists[rvShopLists.Count-1];
    }
    public bool IsRVShopListFinished(int elementIdx)
    {
        return elementIdx >= GetCurRVShopList().Count;
    }

    public Sprite GetCurRVShopListItemImg()
    {
        if (_storage.CurRVShopListIndex >= GetCurRVShopList().Count)
        {
            DebugUtil.LogError(
                $"GetCurRVShopListItemImg out of range index {_storage.CurRVShopListIndex} len {rVshopResources.Count} by list {_storage.CurRVShopListID}");
            return null;
        }

        return ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas,
            rVshopResources[_storage.CurRVShopListIndex].Icon);

        //if (res.RewardType == RVShopRewardType.NodeItem)
        //{
        //    return ResourcesManager.Instance.GetSpriteVariant(AtlasName.UICommon, res.icon);
        //}
        //else
        //return res.GetResImage();
    }

    public bool CheckRVShopListReward(int elementIdx)
    {
        //var reward = AdDataConfigManager.Instance.GetRVShopReward(list_id, list_index);
        if (elementIdx >= GetCurRVShopList().Count)
        {
            return false;
        }

        RVshopResource reward = GetCurRVShopList()[elementIdx];
        return true;
    }

    public bool HasGotInCurRVShopList(int elementIdx)
    {
        return _storage.CurRVShopGotRecord.Contains(elementIdx);
    }

    public bool RVShopFinishItem(RVshopResource resdata, int elementIdx)
    {
        // if ((RVShopRewardType)resdata.RewardType == RVShopRewardType.Resource)
        // {
        //     Gameplay.UserData.Instance.AddRes(resdata.RewardID.ToInt(), resdata.Amount, new GameBIManager.ItemChangeReasonArgs
        //     {
        //         reason =  BiEventAdventureIslandMerge.Types.ItemChangeReason.Rvmarket
        //     });
        //
        // }

        //上次奖励给发但不存储
        if (_storage.CurRVShopGotRecord.Count <= 0 && elementIdx > 0)
        {
            return false;
        }

        int pos = elementIdx;
        _storage.CurRVShopGotRecord.Add(pos);

        do
        {
            pos += 1;
            if (pos >= rVshopResources.Count)
                break;
        } while (!CheckRVShopListReward(pos));

        _storage.CurRVShopListIndex = pos;
        return pos < GetCurRVShopList().Count;
    }

    bool IsNoConstrainRV(string pl)
    {
        //return pl == ConstDefine.RVShop || pl == ConstDefine.PiggyRV || pl == ConstDefine.SpinRV || pl == ConstDefine.GiftRV || pl == ConstDefine.InGameBalloon || pl == ConstDefine.RVMarket || pl == ConstDefine.MasterCardBalloon || pl == ConstDefine.FastCrashTimeUp || pl == ConstDefine.SummerBeachClaimMore || pl == ConstDefine.SummerBeachWarmYourHeart || pl == ConstDefine.RoomFurnitureRV;
        return pl == ADConstDefine.RV_TV_REWARD;
    }

    public void DEBUG_RefreshRVShopList()
    {
        RefreshRVShopList();
    }

    #region 外部调用接口

    /// <summary>
    /// 返回首页检查用
    /// </summary>
    /// <returns></returns>
    public bool Time2PopUpRVShop()
    {
        if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Home &&
            SceneFsm.mInstance.GetCurrSceneType() != StatusType.BackHome)
            return false;
        
        if (ABTestManager.Instance.IsOpenADTest())
            return false;
        
        if (!FunctionsSwitchManager.Instance.FunctionOn(FunctionsSwitchManager.FuncType.RvShop))
            return false;
        
        if (IsRVShopInCd() == false)
            return false;

        if (IsRVShopListFinished(_storage.CurRVShopListIndex))
            return false;

        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, ADConstDefine.RV_TV_REWARD))
            return false;

        Common cdata = AdConfigHandle.Instance.GetCommon();
        if (cdata == null)
            return false;

        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, ADConstDefine.RV_TV_REWARD,
            CommonUtils.GetTimeStamp(), cdata.RvshopCD * 1000);
        return true;
    }

    public bool IsRVShopOpen()
    {
        if (!IsRVShopInCd())
            return false;

        if (IsRVShopListFinished(_storage.CurRVShopListIndex))
            return false;

        return true;
    }


    /// <summary>
    /// 关卡胜利结算调用
    /// </summary>
    /// <param name="passedCount"></param>
    public void LevelPassed4RVShop()
    {
        if (_storage.CurRVShopListID > 0)
        {
            //already opened
            return;
        }

        if (GetCurRVShopList().Count <= 0)
        {
            return;
        }

        if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyRv))
        {
            RefreshRVShopList();
        }
    }

    public bool IsUnLock()
    {
        int level = ExperenceModel.Instance.GetLevel();
        Common cdata = AdConfigHandle.Instance.GetCommon();
        if (cdata.RvshopOpenType == 1)
        {
            if (cdata.RvUnlock <= level)
                return true;
        }

        return false;
    }

    public void PurchaseSuccess(TableShop tableShop)
    {
        if (tableShop == null)
            return;

        var rvShopList = DailyRVModel.Instance.GetCurRVShopList();
        for (int index = 0; index < rvShopList.Count; index++)
        {
            RVshopResource shopData = rvShopList[index];

            if (shopData.ConsumeType != 4 || shopData.ConsumeAmount != tableShop.id)
                continue;
            if( !_storage.GotShopItem.Contains(index))
                _storage.GotShopItem.Add(index);
            var ret = new List<ResData>();
            for (int i = 0; i < shopData.RewardID.Count; i++)
                ret.Add(new ResData(shopData.RewardID[i], shopData.Amount[i]));

            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap);
            reasonArgs.data2 = ADConstDefine.RV_TV_REWARD;
            reasonArgs.data3 = shopData.Id.ToString();
            EventDispatcher.Instance.DispatchEvent(EventEnum.PURCHASE_SUCCESS_REWARD, ret);
            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,
                reasonArgs, () =>
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.RV_SHOP_PURCHASE, tableShop);
                    PayRebateModel.Instance.OnPurchaseAniFinish();
                    PayRebateLocalModel.Instance.OnPurchaseAniFinish();
                });

            UIWindow dailyRv = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIDailyRV);
            if (dailyRv != null)
            {
                CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(dailyRv.canvas.sortingOrder + 1);
            }
            else
            {
              
                DailyRVModel.Instance.RVShopFinishItem(shopData, index);
            }

            return;
        }
    }

    #endregion
}