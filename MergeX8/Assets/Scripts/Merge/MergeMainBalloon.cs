using System;
using System.Collections.Generic;
using ABTest;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using Manager;
using Merge.Order;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Transform))]
public class MergeMainBalloon : MonoBehaviour
{
    private static MergeMainBalloon _instance;

    public static MergeMainBalloon Instance
    {
        get { return _instance; }
    }

    private StorageLuckBallRecord _storageLuckBallRecord;

    public StorageLuckBallRecord StorageLuckBall
    {
        get
        {
            if (_storageLuckBallRecord == null)
                _storageLuckBallRecord = StorageManager.Instance.GetStorage<StorageHome>().LuckBall;

            return _storageLuckBallRecord;
        }
    }

    private Button _balloonButton;
    private Transform _icon1;
    private Image _icon1Image;
    private Transform _icon2;
    private Image _icon2Image1;
    private Image _icon2Image2;
    private LocalizeTextMeshProUGUI _iconText1;
    private LocalizeTextMeshProUGUI _iconText2;
    private LocalizeTextMeshProUGUI _iconText3;
    private List<ResData> _rewardList = new List<ResData>();
    private Transform _balloonBuy;
    private Transform _iconGroupNew;
    private int aMinVlue = 3;
    private int aMaxVlue = 16;
    
    private int cMinVlue = 3;
    private int cMaxVlue = 12;
    private int cPrice = 12;
    bool isPayShow = false;
    private BalloonPack _balloonPack;
    public void Check(bool isDebug = false)
    {
        if (!CanShowUI() && !isDebug)
            return;
       
        MergeManager.Instance.mergeItemCountsDirty = true;
        Dictionary<int, int> mergeItemCounts = MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.Main);
        for (int i = 0; i < MergeManager.Instance.GetBagCount(MergeBoardEnum.Main); i++)
        {
            StorageMergeItem mergeItem = MergeManager.Instance.GetBagItem(i,MergeBoardEnum.Main);
            if (mergeItem == null)
                continue;

            if (mergeItemCounts.ContainsKey(mergeItem.Id))
                mergeItemCounts[mergeItem.Id] += 1;
            else
            {
                mergeItemCounts[mergeItem.Id] = 1;
            }
        }

        foreach (var mergeItem in MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Rewards)
        {
            if (mergeItem == null)
                continue;

            if (mergeItemCounts.ContainsKey(mergeItem.Id))
                mergeItemCounts[mergeItem.Id] += 1;
            else
            {
                mergeItemCounts[mergeItem.Id] = 1;
            }
        }
        MergeManager.Instance.mergeItemCountsDirty = true;
        Common common = AdConfigHandle.Instance.GetCommon();
        if (common.LuckyBalloonValue1 != null && common.LuckyBalloonValue1.Count > 1)
        {
            aMinVlue = common.LuckyBalloonValue1[0];
            aMaxVlue = common.LuckyBalloonValue1[1];
        }
        if (common.LuckyBalloonValue2!= null && common.LuckyBalloonValue2.Count > 1)
        {
            cMinVlue = common.LuckyBalloonValue2[0];
            cMaxVlue = common.LuckyBalloonValue2[1];
        }
        if (common.LuckyBalloonPrice != null)
            cPrice = common.LuckyBalloonPrice;
        
        var resData = Plan_A(mergeItemCounts);
        if (resData == null)
        {
            var ratio = AdConfigHandle.Instance.GetLuckBalloonResRatio();
            if(Random.Range(0, 100) < ratio)
                resData = Plan_B(mergeItemCounts);
        }
        
        if(resData == null)
            resData = Plan_C(mergeItemCounts);
        
        if(resData == null)
            return;
        
        _rewardList.Clear();
        _rewardList.Add(resData);
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        isPayShow = false;
        if (!CommonUtils.IsSameDayWithToday((ulong) storageHome.Balloon.LastShowTime))
        {
             storageHome.Balloon.PayCount = 0;
             storageHome.Balloon.PayShowCount = 0;
        }
        bool canShowRvorExtend = true;
        if (IsFailedWithinLimit())
            canShowRvorExtend= false;
        int balloonData = AdConfigHandle.Instance.GetLuckBalloonData();
        if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BALLOON) &&
            !ConsumeExtendManager.Instance.CanShowConsumeExtend(balloonData))
            canShowRvorExtend= false;
        //RV次数没用完不单独展示钻石气球
        if (!AdSubSystem.Instance.IsFailedByRvConfiged(ADConstDefine.RV_BALLOON) &&
            !AdSubSystem.Instance.IsFailedByRVWithinLimit(ADConstDefine.RV_BALLOON))
        {
            if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BALLOON))
                canShowRvorExtend= false;
        }
        
       var itemConfig= GameConfigManager.Instance.GetItemConfig(resData.id);
        var packs = AdConfigHandle.Instance.GetBallPack();
        if (packs != null && packs.Count > 0)
        { 
            _balloonPack = packs.Find(a => a.Merge_line == itemConfig.in_line);
            if(_balloonPack==null)
                _balloonPack=packs.RandomPickOne();
            if (!canShowRvorExtend || (storageHome.Balloon.ShowCount % (_balloonPack.Interval + 1) == 0 &&
                                       storageHome.Balloon.PayShowCount <= _balloonPack.Max_times &&
                                       storageHome.Balloon.PayCount <= _balloonPack.Pay_times))
            {
                isPayShow = true;
                _rewardList.Clear();
                storageHome.Balloon.PayShowCount++;
                for (int i = 0; i < _balloonPack.Content.Count; i++)
                {
                    _rewardList.Add(new ResData(_balloonPack.Content[i],_balloonPack.Count[i]));
                }
            }
        }





        storageHome.Balloon.LastShowTime = (long) APIManager.Instance.GetServerTime();
        storageHome.Balloon.ShowCount++;
        TryShow(isPayShow);
    }

    private ResData Plan_A(Dictionary<int, int> mergeItemCounts)
    {
        List<TableMergeItem> mergeItems = new List<TableMergeItem>();
        List<TableMergeItem> taskNeedItem = new List<TableMergeItem>();
        foreach (var taskItem in MainOrderManager.Instance.CurTaskList)
        {
            int index = 0;
            foreach (var itemId in taskItem.ItemIds)
            {
                TableMergeLine mergeLine = MergeConfigManager.Instance.GetMergeLine(itemId);
                if (mergeLine == null)
                {
                    index++;
                    continue;
                }
                
                foreach (var inLineId in mergeLine.output)
                {
                    TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(inLineId);
                    if(itemConfig == null)
                        continue;
                    
                    if (itemConfig.price < aMinVlue || itemConfig.price >aMaxVlue)
                        continue;
                    
                    if(!mergeItemCounts.ContainsKey(itemConfig.id))
                        continue;
                    
                    if(mergeItemCounts[itemConfig.id]%2 == 0)
                        continue;
                    
                    if(mergeItemCounts.ContainsKey(itemId) && mergeItemCounts[itemId] >= taskItem.ItemNums[index])
                    {
                        taskNeedItem.Add(itemConfig);
                    }
                    else
                    {
                        mergeItems.Add(itemConfig);
                    }
                }
                index++;
            }
        }
        
        if (mergeItems.Count > 0)
            return new ResData(mergeItems.RandomPickOne().id, 1);
        
        if (taskNeedItem.Count > 0)
            return new ResData(taskNeedItem.RandomPickOne().id, 1);

        return null;
    }

    private ResData Plan_B(Dictionary<int, int> mergeItemCounts)
    {
        var ids = AdConfigHandle.Instance.GetLuckBalloonResIds();
        if (ids == null || ids.Count == 0)
            return null;
        
        List<TableMergeItem> mergeItems = new List<TableMergeItem>();
        foreach (var itemId in ids)
        {   
            TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(itemId);
            if(itemConfig==null)
                continue;
            
            if(!mergeItemCounts.ContainsKey(itemConfig.id))
                continue;
                    
            if(mergeItemCounts[itemConfig.id]%2 == 0)
                continue;
            
            mergeItems.Add(itemConfig);
        }
        
        if (mergeItems.Count <= 0)
            return null;
        
        return new ResData(mergeItems.RandomPickOne().id, 1);
    }

    private ResData Plan_C(Dictionary<int, int> mergeItemCounts)
    {
        List<TableMergeItem> mergeItems=new List<TableMergeItem>();
        List<TableMergeItem> taskNeedItem = new List<TableMergeItem>();
        foreach (var taskItem in MainOrderManager.Instance.CurTaskList)
        {
            int index = 0;
            foreach (var itemId in taskItem.ItemIds)
            {
                TableMergeLine mergeLine = MergeConfigManager.Instance.GetMergeLine(itemId);
                if (mergeLine == null)
                {
                    index++;
                    continue;
                }
                foreach (var inLineId in mergeLine.output)
                {
                    TableMergeItem itemConfig = GameConfigManager.Instance.GetItemConfig(inLineId);
                    if (itemConfig!=null && itemConfig.price >= cMinVlue && itemConfig.price <= cMaxVlue)
                    {
                        if(mergeItemCounts.ContainsKey(itemId) && mergeItemCounts[itemId] >= taskItem.ItemNums[index])
                        {
                            taskNeedItem.Add(itemConfig);
                        }
                        else
                        {
                            mergeItems.Add(itemConfig);
                        }
                    }
                }
                index++;
            }
        }

        List<TableMergeItem> items = null;
        if (mergeItems.Count > 0)
            items = mergeItems;
        else if(taskNeedItem.Count > 0)
            items = taskNeedItem;

        if (items == null || items.Count == 0)
            return null;
        
        var randomItem=  items.RandomPickOne();
        int count = Math.Max(1, cPrice / randomItem.price);
        
        return new ResData(randomItem.id, count);
    }
    
    private float startPosY = 400;
    private float endPosY = -1800;
    public bool isMoving = false;

    public void Init()
    {
        _instance = this;
        _balloonButton = transform.Find("ButtonTargetGraphic").GetComponent<Button>();
        _balloonButton.onClick.AddListener((() =>
        {
            transform.DOKill();
            UIPopupLuckyBalloonController.ShowUI(_rewardList, (bool isRv) =>
            {
                if (isRv)
                    Reset();
                else
                {
                    DoMove();
                }
            },isPayShow,_balloonPack);
        }));
        transform.localPosition = new Vector3(transform.localPosition.x, startPosY, 0);
        _icon1 = transform.Find("BoxGroup/IconGroup/Icon1");
        _icon2 = transform.Find("BoxGroup/IconGroup/Icon2");
        _icon1Image = transform.Find("BoxGroup/IconGroup/Icon1/Image").GetComponent<Image>();
        _icon2Image1 = transform.Find("BoxGroup/IconGroup/Icon2/Image1").GetComponent<Image>();
        _icon2Image2 = transform.Find("BoxGroup/IconGroup/Icon2/Image2").GetComponent<Image>();
        _balloonBuy = transform.Find("BalloonBuy");
        _iconText1 = transform.Find("BoxGroup/IconGroup/Icon1/Image/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _iconText2 = transform.Find("BoxGroup/IconGroup/Icon2/Image1/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _iconText3 = transform.Find("BoxGroup/IconGroup/Icon2/Image2/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _iconGroupNew=transform.Find("BoxGroup/IconGroupNew");
        // EventDispatcher.Instance.AddEventListener(MergeEvent.BALLOON_RESET, OnReset);

        
    }

    private void OnReset(BaseEvent obj)
    {
        Reset();
    }

    public void Reset()
    {
        DebugUtil.Log("------------------MERGEMAINBALL RESET------------------");
        isMoving = false;
        transform.DOKill();
        transform.localPosition = new Vector3(transform.localPosition.x, startPosY, 0);
        transform.gameObject.SetActive(false);
    }

    public bool IsFailedWithinLimit()
    {
        var common = AdConfigHandle.Instance.GetCommon();
        if (common == null)
            return false;
        if (!CommonUtils.IsSameDayWithToday((ulong) StorageLuckBall.LastPlayTime))
        {
            StorageLuckBall.PlayCount = 0;
            StorageLuckBall.LastPlayTime = 0;
        }

        if (common.LuckyBalloonLimit <= StorageLuckBall.PlayCount)
        {
            //Debug.Log("----Ball-----IsFailedWithinLimit ---Limit=" + common.LuckyBalloonLimit + " PlayCount+ " +StorageLuckBall.PlayCount);

            return true;
        }

        return false;
    }

    public bool CanShowUI()
    {      
        if (ABTestManager.Instance.IsOpenADTest())
            return false;
        
        if (!FunctionsSwitchManager.Instance.FunctionOn(FunctionsSwitchManager.FuncType.RvBalloon))
            return false;
        
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Balloon))
            return false;
  
        if (GuideSubSystem.Instance.IsShowingGuide())
            return false;
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, ADConstDefine.RV_BALLOON))
            return false;

        if (isMoving)
            return false;
        
        
        var packs = AdConfigHandle.Instance.GetBallPack();
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        if (packs != null && packs.Count > 0)
        {
            _balloonPack=packs.RandomPickOne();
            if (!CommonUtils.IsSameDayWithToday((ulong) storageHome.Balloon.LastShowTime))
            {
                storageHome.Balloon.PayCount = 0;
                storageHome.Balloon.PayShowCount = 0;
            }
            if (storageHome.Balloon.PayShowCount <= _balloonPack.Max_times && storageHome.Balloon.PayCount <= _balloonPack.Pay_times)
            {
                var cd1 = GlobalConfigManager.Instance.GetNumValue("ballon_cd")*1000;
                CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, ADConstDefine.RV_BALLOON, CommonUtils.GetTimeStamp(),cd1);
                return true;
            }
        }
        
        if (IsFailedWithinLimit())
            return false;
        int balloonData = AdConfigHandle.Instance.GetLuckBalloonData();
        if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BALLOON) &&
            !ConsumeExtendManager.Instance.CanShowConsumeExtend(balloonData))
            return false;
        //RV次数没用完不单独展示钻石气球
        if (!AdSubSystem.Instance.IsFailedByRvConfiged(ADConstDefine.RV_BALLOON) &&
            !AdSubSystem.Instance.IsFailedByRVWithinLimit(ADConstDefine.RV_BALLOON))
        {
            if (!AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_BALLOON))
                return false;
        }

    
        var cd = GlobalConfigManager.Instance.GetNumValue("ballon_cd")*1000;
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, ADConstDefine.RV_BALLOON, CommonUtils.GetTimeStamp(),cd);
        return true;
    }


    public void TryShow(bool isPayShow)
    {
        _balloonBuy.gameObject.SetActive(isPayShow);
        _icon1.parent.gameObject.SetActive(!isPayShow);
        _iconGroupNew.gameObject.SetActive(isPayShow);
        if (_rewardList == null || _rewardList.Count <= 0)
            return;
        if (_rewardList.Count == 1)
        {
            _icon1.gameObject.SetActive(true);
            _icon2.gameObject.SetActive(false);
            var config = GameConfigManager.Instance.GetItemConfig(_rewardList[0].id);
            if (config != null)
                _icon1Image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(config.image);
            
            _iconText1.gameObject.SetActive(_rewardList[0].count > 1);
            _iconText1.SetText("x"+_rewardList[0].count.ToString());
        }
        else
        {
            _icon1.gameObject.SetActive(false);
            _icon2.gameObject.SetActive(true);
            var config = GameConfigManager.Instance.GetItemConfig(_rewardList[0].id);
            if (config != null)
                _icon2Image1.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(config.image);
            config = GameConfigManager.Instance.GetItemConfig(_rewardList[1].id);
            if (config != null)
                _icon2Image2.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(config.image);
            
            _iconText2.SetText("x"+_rewardList[0].count.ToString());
            _iconText3.SetText("x"+_rewardList[1].count.ToString());
            
            _iconText2.gameObject.SetActive(_rewardList[0].count > 1);
            _iconText3.gameObject.SetActive(_rewardList[1].count > 1);
        }

        Reset();
        DoMove();
    }

    public void DoMove()
    {
        isMoving = true;
        transform.gameObject.SetActive(true);
        transform.DOLocalMoveY(endPosY, 60).onComplete = () => { isMoving = false; };
    }

    public void DebugShowPay(int packId)
    {
        
        var packs = AdConfigExtendConfigManager.Instance.GetConfig<BalloonPack>();
        if (packs != null && packs.Count > 0)
        { 
            _balloonPack = packs.Find(a => a.Id == packId);
            if(_balloonPack==null)
                _balloonPack=packs.RandomPickOne();
           
            isPayShow = true;
            _rewardList.Clear();
            for (int i = 0; i < _balloonPack.Content.Count; i++)
            {
                _rewardList.Add(new ResData(_balloonPack.Content[i],_balloonPack.Count[i]));
            }
        }
        TryShow(isPayShow);
    }
}