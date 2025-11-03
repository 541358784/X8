using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupIcebreakingPackController : UIWindowController
{
    private Button closeButton;
    private static string coolTimeKey = "IceBreakPack";
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
        var configs = IcebreakingPackModel.Instance.GetIceBreakingPacks();
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
        int leftTime = IcebreakingPackModel.Instance.GetPackCoolTime() * 1000;
        _timeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
    }
    public static bool CanShowUIWithOpenWindow()
    {
        if (!CanShowUIWithOutOpenWindow())
            return false;
        UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPack, "auto_main");

        return true;
    }
    public static bool CanShowUIWithOutOpenWindow()
    {
        if (IcebreakingPackModel.Instance.IsLock())
            return false;
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.IceBreak))
            return false;
        // if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        //     return false;

        if (IcebreakingPackModel.Instance.packData.IsFinish)
            return false;
        var configs = IcebreakingPackModel.Instance.GetIceBreakingPacks();
        if (configs == null || configs.Count <= 0)
            return false;
        // IcebreakingPackModel.Instance.RecordOpenState();
        // int cd = GlobalConfigManager.Instance.GetNumValue("icebreak_pack_auto");
        // CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
        //     CommonUtils.GetTimeStamp());
        //
        // UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPack, "auto_main");

        return true;
    }
    
    public static bool CanShowUI()
    {
        if (IcebreakingPackModel.Instance.IsLock())
            return false;
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.IceBreak))
            return false;
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;

        if (IcebreakingPackModel.Instance.packData.IsFinish)
            return false;
        var configs = IcebreakingPackModel.Instance.GetIceBreakingPacks();
        if (configs == null || configs.Count <= 0)
            return false;
        IcebreakingPackModel.Instance.RecordOpenState();
        int cd = GlobalConfigManager.Instance.GetNumValue("icebreak_pack_auto");
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
            CommonUtils.GetTimeStamp());

        UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPack, "auto_main");

        return true;
    }
    public static bool CanShowUILevelUp()
    {
        if (IcebreakingPackModel.Instance.IsLock())
            return false;
        int unlockLevel = GlobalConfigManager.Instance.GetNumValue("icebreak_pack_unlock");
        if (ExperenceModel.Instance.GetLevel() != unlockLevel)
            return false;
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;

        if (IcebreakingPackModel.Instance.packData.IsFinish)
            return false;
        var configs = IcebreakingPackModel.Instance.GetIceBreakingPacks();
        if (configs == null || configs.Count <= 0)
            return false;
        IcebreakingPackModel.Instance.RecordOpenState();
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());

        UIManager.Instance.OpenUI(UINameConst.UIPopupIcebreakingPack, "auto_main");
        return true;
    }
}