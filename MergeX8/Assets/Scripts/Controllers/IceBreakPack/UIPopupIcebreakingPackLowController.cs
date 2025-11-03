using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupIcebreakingPackLowController : UIWindowController
{
    private Button closeButton;
    private static string coolTimeKey = "IceBreakPackLow";
    private Animator _animator;
    private string source = null;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _titleText;

    public override void PrivateAwake()
    {
        closeButton = transform.Find("Root/CloseButton").GetComponent<Button>();
        _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _titleText = transform.Find("Root/BGGroup/TitleGroup/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        closeButton.onClick.AddListener(OnBtnClose);
        _animator = gameObject.GetComponent<Animator>();
        InvokeRepeating("UpdateTimeText", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.IceBreak_Pack_Finish, OnFinish);

    }

    private void OnFinish(BaseEvent obj)
    {
        CloseWindowWithinUIMgr(true);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        if (objs != null && objs.Length > 0)
            source = objs[0] as string;
        var configs = IcebreakingPackSecondModel.Instance.GetIceBreakingPacks(3);
        if (configs == null)
            return;

        var rewardItems = transform.Find("Root/RewardItem").gameObject;
        var packItem=rewardItems.AddComponent<UIIcebreakingPackItem>();
        packItem.Init(configs[0], source);
        _titleText.SetTerm(configs[0].Name);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPackOpen, source,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "IceBreak");
    }

    private void OnBtnClose()
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventPackClose, source,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), "IceBreak");
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.IceBreak_Pack_Finish, OnFinish);
    }

    public void UpdateTimeText()
    {
        int leftTime = IcebreakingPackSecondModel.Instance.GetPackCoolTime() * 1000;
        _timeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
    }
    public static bool CanShowUIWithOpenWindow()
    {
        if (!CanShowUIWithOutOpenWindow())
            return false;
        UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPackLow, "auto_main");
        return true;
    }
    public static bool CanShowUIWithOutOpenWindow()
    {
        if (IcebreakingPackModel.Instance.IsLock())
            return false;
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.IceBreak))
            return false;
        var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        if (storageCommon.RevenueCount > 0) //有过充值
            return false;
        if (!IcebreakingPackModel.Instance.packData.IsFinish)
            return false;
        if (IcebreakingPackSecondModel.Instance.packData.IsFinish)
            return false;
        var cfgs = IcebreakingPackModel.Instance.GetIceBreakingPacks();
        if (cfgs == null || cfgs.Count <= 0)
            return false;
        int leftTime = IcebreakingPackSecondModel.Instance.GetPackCoolTime(3);
        if (leftTime <=0)
            return false;
        var configs = IcebreakingPackSecondModel.Instance.GetIceBreakingPacks(3);
        if (configs == null || configs.Count <= 0)
            return false;
        // UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPackLow, "auto_main");
        return true;
    }
    public static bool CanShowUI()
    {
        if (IcebreakingPackModel.Instance.IsLock())
            return false;
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.IceBreak))
            return false;
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, coolTimeKey))
            return false;
        var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        if (storageCommon.RevenueCount > 0) //有过充值
            return false;

        if (!IcebreakingPackModel.Instance.packData.IsFinish)
            return false;
        if (IcebreakingPackSecondModel.Instance.packData.IsFinish)
            return false;

        var cfgs = IcebreakingPackModel.Instance.GetIceBreakingPacks();
        if (cfgs == null || cfgs.Count <= 0)
            return false;
        var PackCfg = cfgs[0];
        long finishTime = IcebreakingPackModel.Instance.packData.FinishTime > 0
            ? IcebreakingPackModel.Instance.packData.FinishTime
            : IcebreakingPackModel.Instance.packData.StartTime + PackCfg.Duration;
        long leftTime =finishTime+24*60*60 - CommonUtils.GetTimeStamp()/1000;
        if (leftTime >0)
        {
            return false;
        }
        var configs = IcebreakingPackSecondModel.Instance.GetIceBreakingPacks(3);
        if (configs == null || configs.Count <= 0)
            return false;
        
        IcebreakingPackSecondModel.Instance.RecordOpenState();
        int cd = GlobalConfigManager.Instance.GetNumValue("icebreak_pack_auto");
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey,
            CommonUtils.GetTimeStamp(), cd * 60 * 1000);
        UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPackLow, "auto_main");
     
        return true;
    }
}