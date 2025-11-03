using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using GamePool;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using GamePool;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupNewDailyGiftController : UIWindowController
{
    private Transform rewardParent;
    private Button closeButton;
    public static string coolTimeKey = "NewDailyPack";
    private Animator _animator;
    private string source = null;
    private LocalizeTextMeshProUGUI _timeText;
    private List<UINewDailNewyBundleItem> _bundleItems;
    public override void PrivateAwake()
    {
        _bundleItems = new List<UINewDailNewyBundleItem>();
        closeButton = transform.Find("Root/CloseButton").GetComponent<Button>();
        _timeText = transform.Find("Root/TopGroup/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        closeButton.onClick.AddListener(OnBtnClose);
        _animator = gameObject.GetComponent<Animator>();
        InvokeRepeating("UpdateTimeText", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.NewDaily_Pack_Finish, OnFinish);
        EventDispatcher.Instance.AddEventListener(EventEnum.NewDaily_Pack_Time_REFRESH, OnTimeFinish);
    }

    private void OnTimeFinish(BaseEvent obj)
    {
        var packInfo = NewDailyPackModel.Instance.packData.PackInfo;
        for (int i = 0; i < _bundleItems.Count; i++)
        {
             var pi = GlobalConfigManager.Instance.GetNewDailyPackPackageConfig(packInfo[i].PackInfoId);
             _bundleItems[i].Init(packInfo[i],pi,source);
        }
     
    }

    private void OnFinish(BaseEvent obj)
    {
        CloseWindowWithinUIMgr(true);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        if (objs != null && objs.Length > 0)
            source = objs[0] as string;
            
        rewardParent = transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content");
        var rewardItems = rewardParent.Find("RewardItem1");
        rewardItems.gameObject.SetActive(false);
        var packInfo = NewDailyPackModel.Instance.packData.PackInfo;
        for (int i = 0; i < packInfo.Count; i++)
        {
            var packItem = Instantiate(rewardItems,rewardParent).gameObject.AddComponent<UINewDailNewyBundleItem>();
            packItem.transform.localScale=Vector3.one;
            packItem.gameObject.SetActive(true);
            var pi = GlobalConfigManager.Instance.GetNewDailyPackPackageConfig(packInfo[i].PackInfoId);
            packItem.Init(packInfo[i], pi, source);
            _bundleItems.Add(packItem);
        }

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDailyDealsPop);
    }

    private void OnBtnClose()
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));

    }
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NewDaily_Pack_Finish, OnFinish);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NewDaily_Pack_Time_REFRESH, OnTimeFinish);
    }

    public void UpdateTimeText()
    {
        int leftTime = NewDailyPackModel.Instance.GetPackCoolTime() * 1000;
        if(leftTime<=0)
            NewDailyPackModel.Instance.RefreshPack();
        _timeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
    }

    public static bool CanShowUIWithOpenWindow()
    {
        if (!CanShowUIWithOutOpenWindow())
            return false;
        if (!UIManager.Instance.GetOpenedUIByPath<UIPopupNewDailyGiftController>(UINameConst.UIPopupNewDailyGift))
            UIManager.Instance.OpenUI(UINameConst.UIPopupNewDailyGift, "pack");
        return true;
    }
    public static bool CanShowUIWithOutOpenWindow()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.NewDailyPack))
        {
            return false;
        }
        var common = AdConfigHandle.Instance.GetCommon();
        if (common.NewDailyPackPrice < 0)
            return false;
 
        // if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, coolTimeKey))
        //     return false;
        NewDailyPackModel.Instance.RefreshPack();
        if (!NewDailyPackModel.Instance.IsHaveConifg())
            return false;
        if (NewDailyPackModel.Instance.IsFinish())
            return false;
        // if (NewDailyPackModel.Instance.GetPopTimes() >= common.DailyPackTimes)
        //     return false;
        // CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey,
        //     CommonUtils.GetTimeStamp(),common.NewDailyPackInterval*60 * 1000);
        //
        // NewDailyPackModel.Instance.RecordOpenState();
        // UIManager.Instance.OpenUI(UINameConst.UINewDailyPack, "pack");
        return true;
    }
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.NewDailyPack))
        {
            return false;
        }
        var common = AdConfigHandle.Instance.GetCommon();
        if (common.NewDailyPackPrice < 0)
            return false;
 
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, coolTimeKey))
            return false;
        NewDailyPackModel.Instance.RefreshPack();
        if (!NewDailyPackModel.Instance.IsHaveConifg())
            return false;
        if (NewDailyPackModel.Instance.IsFinish())
            return false;
        if (NewDailyPackModel.Instance.GetPopTimes() >= common.DailyPackTimes)
            return false;

        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey,
            CommonUtils.GetTimeStamp(),common.NewDailyPackInterval*60 * 1000);

        NewDailyPackModel.Instance.RecordOpenState();
        UIManager.Instance.OpenUI(UINameConst.UIPopupNewDailyGift, "pack");
        return true;
    }
    
    public static bool CanShowUILevelUp()
    {
        int unlockLevel = GlobalConfigManager.Instance.GetNumValue("new_daily_pack_unlock");
        if (ExperenceModel.Instance.GetLevel() != unlockLevel)
            return false;
        var common = AdConfigHandle.Instance.GetCommon();
        if (common.NewDailyPackPrice < 0)
            return false;
 
        // if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, coolTimeKey))
        //     return false;
        NewDailyPackModel.Instance.RefreshPack();
        // if (!NewDailyPackModel.Instance.IsHaveConifg())
        //     return false;
        // if (NewDailyPackModel.Instance.IsFinish())
        //     return false;
        // if (NewDailyPackModel.Instance.GetPopTimes() >= common.DailyPackTimes)
        //     return false;

        // CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey,
        //     CommonUtils.GetTimeStamp(),common.NewDailyPackInterval*60 * 1000);
        //
        // NewDailyPackModel.Instance.RecordOpenState();
        // UIManager.Instance.OpenUI(UINameConst.UINewDailyPack, "pack");
        return false;
    }
}