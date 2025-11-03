using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UIEasterPackController : UIWindowController
{
    private Transform rewardParent;
    private Button closeButton;
    private static string coolTimeKey = "EasterGift";
    private Animator _animator;
    private string source = null;
    private LocalizeTextMeshProUGUI _timeText;

    public override void PrivateAwake()
    {
        closeButton = transform.Find("Root/CloseButton").GetComponent<Button>();
        _timeText = transform.Find("Root/TopGroup/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        closeButton.onClick.AddListener(OnBtnClose);
        _animator = gameObject.GetComponent<Animator>();
        InvokeRepeating("UpdateTimeText", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.EASTER_PACK_Finish, OnFinish);
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
        var packInfo =EasterGiftModel.Instance.GetEasterBundleConfig();

        for (int i = 0; i < packInfo.Count; i++)
        {
            var packItem = Instantiate(rewardItems.gameObject, rewardParent).AddComponent<UIEasterGiftItem>();
            packItem.gameObject.SetActive(true);
          
            packItem.Init(packInfo[i], source);
        }


    }

    private void OnBtnClose()
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.EASTER_PACK_Finish, OnFinish);
    }

    public void UpdateTimeText()
    {
        _timeText.SetText(EasterGiftModel.Instance.GetActivityRewardLeftTimeString());
    }

    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Easter))
        {
            return false;
        }
        if (!EasterModel.Instance.IsStart())
            return false;
          if (!EasterGiftModel.Instance.IsOpened())
            return false;
  
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;

        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
            CommonUtils.GetTimeStamp());

        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPackagePointPop);
        UIManager.Instance.OpenUI(UINameConst.UIEasterPack, "auto_main");
        return true;
    }
}