using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CardCollection;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public class CardCollectionActivityModel :ActivityEntityBase
{
    private static CardCollectionActivityModel _instance;
    public static CardCollectionActivityModel Instance => _instance ?? (_instance = new CardCollectionActivityModel());
    
    public override string Guid => "OPS_EVENT_TYPE_CARD_COLLECTION";
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAuto()
    {
        Instance.Init();
    }

    public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
        ulong rewardEndTime, bool manualEnd, string configJson, string activitySubType)
    {
        CardCollectionConfigManager.Instance.InitConfig(configJson);
        base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
            activitySubType);
        InitServerDataFinish();
        _lastActivityOpenState = CardCollectionActivityModel.Instance.IsOpened();
        DebugUtil.Log($"InitConfig:{Guid}");
        if (!CardCollectionModel.Instance.StorageCardCollection.OpenThemeList.Contains(CurStorage.ThemeId))
        {
            CardCollectionModel.Instance.StorageCardCollection.OpenThemeList.Add(CurStorage.ThemeId);
        }

        var theme = CardCollectionModel.Instance.GetCardThemeState(CurStorage.ThemeId);
        theme.TryDownloadRes();
        if (theme.CardThemeConfig.UpGradeTheme > 0)
        {
            var upgradeTheme = CardCollectionModel.Instance.GetCardThemeState(theme.CardThemeConfig.UpGradeTheme);
            upgradeTheme.TryDownloadRes();
        }
        if (!CardCollectionModel.Instance.StorageCardCollection.IsInit)
        {
            CardCollectionModel.Instance.StorageCardCollection.IsInit = true;
            foreach (var pair in CardCollectionModel.Instance.StorageCardCollection.CollectedCards)
            {
                var cardStorage = pair.Value;
                var cardState = CardCollectionModel.Instance.GetCardItemState(pair.Key);
                if (cardState.CardBookStateList[0].CardThemeStateList[0].CardThemeConfig.Id == CurStorage.ThemeId)
                    continue;
                var consumeCount = cardStorage.Count - cardStorage.ConsumeCount - 1;
                if (consumeCount>0)
                {
                    cardState.ConsumeCard(consumeCount);
                }
            }
        }
        CardCollectionModel.Instance.CollectAbstractCardPackage();
        var themeInUse = CardCollectionModel.Instance.GetCardThemeState(CurStorage.ThemeId);
        if (themeInUse != null)
        {
            themeInUse.GetUpGradeTheme().CreateTaskEntrance();
            themeInUse.GetUpGradeTheme().CreateAuxEntrance();
        }
    }
    public CardCollectionActivityModel()
    {
        TMatch.Timer.Register(1, UpdateTime, null, true);
    }
    public override bool IsOpened(bool hasLog = false)
    {
        return UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CardCollection) //已解锁
               && base.IsOpened(hasLog) && CurStorage != null && 
               CardCollectionModel.Instance.IsResReady(CardCollectionModel.Instance.GetCardThemeState(CurStorage.ThemeId));
    }
    
    public CardCollectionCardThemeState GetThemeOnOpened()
    {
        if (!IsOpened())
            return null;
        return CardCollectionModel.Instance.GetCardThemeState(CurStorage.ThemeId);
    }

    public int CurrentThemeId()
    {
        return CurStorage.ThemeId;
    }
    
    public void EndActivity()
    {
        //关闭兑换商店弹窗
        var storePopup = UIManager.Instance.GetOpenedUIByPath<UICardGiftController>(UINameConst.UIMainCardGift);
        if (storePopup)
            storePopup.AnimCloseWindow();
    }

    public void StartActivity()
    {
        
    }

    private bool _lastActivityOpenState;
    public void UpdateTime()
    {
        if (!IsInitFromServer())
            return;
        var currentActivityOpenState = CardCollectionActivityModel.Instance.IsOpened();
        if (_lastActivityOpenState == currentActivityOpenState)
            return;
        if (_lastActivityOpenState && !currentActivityOpenState)
        {
            CardCollectionActivityModel.Instance.EndActivity();
        }
        else if(!_lastActivityOpenState && currentActivityOpenState)
        {
            CardCollectionActivityModel.Instance.StartActivity();
        }

        _lastActivityOpenState = currentActivityOpenState;
    }

    public override bool CanDownLoadRes()
    {
        return UnlockManager.IsUnlockSoon(UnlockManager.MergeUnlockType.CardCollection);
    }

    public Dictionary<string, StorageCardCollectionActivity> Storage =>
        StorageManager.Instance.GetStorage<StorageHome>().CardCollectionActivity;
    public StorageCardCollectionActivity CurStorage
    {
        get
        {
            if (!IsInitFromServer())
                return null;
            var config = CardCollectionConfigManager.Instance.GetConfig<CardCollectionThemeConfig>()[0];
            if (!Storage.TryGetValue(ActivityId, out var curStorage))
            {
                curStorage = new StorageCardCollectionActivity()
                {
                    ActivityId = ActivityId,
                    EndTime = (long) EndTime,
                    StartTime = (long) StartTime,
                    IsStart = false,
                    ThemeId = config.ThemeId,
                    FreeWeightTime1 = (long) (StartTime + (ulong)config.WeightTime1 * XUtility.Hour),
                };
                for (var i = 0; i < CardCollectionModel.Instance.StorageCardCollection.UnOpenPackageList.Count; i++)
                {
                    if (CardCollectionModel.Instance.StorageCardCollection.UnOpenPackageList[i].Id == 999999)
                    {
                        CardCollectionModel.Instance.StorageCardCollection.UnOpenPackageList.RemoveAt(i);
                        i--;
                    }
                }
                
                Storage.Add(curStorage.ActivityId,curStorage);
            }
            else
            {
                curStorage.ThemeId = config.ThemeId;
                curStorage.FreeWeightTime1 = (long)(StartTime + (ulong)config.WeightTime1 * XUtility.Hour);
            }
            return curStorage;
        }
    }

    public static bool CanShowStart()
    {
        if (Instance.IsOpened() && 
            !Instance.CurStorage.IsStart && 
            (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
             SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome) &&
            !GuideSubSystem.Instance.IsShowingGuide())
        {
            Instance.CurStorage.IsStart = true;
            UIPopupCardStartController.Open(Instance.CurStorage);
            return true;
        }
        return false;
    }
}