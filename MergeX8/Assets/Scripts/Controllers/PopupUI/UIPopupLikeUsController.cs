using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using System.Collections;
using DragonPlus.Config;
using DragonU3DSDK.Network.API.Protocol;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupLikeUsController : UIWindowController
{
    public static string coolTimeKey = "LikeUsTime";
    private LocalizeTextMeshProUGUI _rewardText;
    private Button ButtonLaterButton { get; set; }
    private Button ButtonFirstButton { get; set; }
    private Button ButtonCloseButton { get; set; }

    public Action OnClose = null;

    private Animator _animator;

    public override void PrivateAwake()
    {
        ButtonLaterButton = transform.Find("Root/LaterButton").GetComponent<Button>();
        ButtonFirstButton = transform.Find("Root/FirstButton").GetComponent<Button>();
        ButtonCloseButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        _rewardText = transform.Find("Root/MiddleGroup/BubbleGroup/RewardText").GetComponent<LocalizeTextMeshProUGUI>();

        ButtonLaterButton.onClick.AddListener(OnButtonLaterButtonClick);
        ButtonFirstButton.onClick.AddListener(OnButtonFirstButtonClick);
        ButtonCloseButton.onClick.AddListener(OnButtonCloseClick);
        _animator = transform.GetComponent<Animator>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
            CommonUtils.GetTimeStamp());
        var rewardList =
            RewardModal.Instance.ParseReward(
                GlobalConfigManager.Instance.GetGlobal_Config_Number_Value("LikeUs_reward"));
        if (rewardList != null && rewardList.Count > 0)
            _rewardText.SetText("" + rewardList[0].count);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLikeUsPop);
    }

    private void OnButtonCloseClick()
    {
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.LikeUsFinish = true;
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLikeUsLater, "quit");
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    private void OnButtonLaterButtonClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLikeUsLater, "later");
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));

        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.LikeUsFinish = true;
    }

    private void OnButtonFirstButtonClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        storage.LikeUsFinish = true;

        string likeUsUrl =
            GlobalConfigManager.Instance.GetGlobal_Config_Number_Value(GlobalStringConfigKey.like_us_facebook_url);
        Application.OpenURL(likeUsUrl);

        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLikeUsLike, "likeusnow");
        string reward = GlobalConfigManager.Instance.GetGlobal_Config_Number_Value("LikeUs_reward");
        if (reward == null || reward == "")
            return;
        var rewardList = RewardModal.Instance.ParseReward(reward);
        CommonRewardManager.Instance.PopCommonReward(rewardList, CurrencyGroupManager.Instance.currencyController, true,
            new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.LikeUs));
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        OnClose?.Invoke();
    }

    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.LikeUs))
            return false;

        StorageHome storage = StorageManager.Instance.GetStorage<StorageHome>();
        if (storage == null)
            return false;

        if (storage.LikeUsFinish)
            return false;

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupLikeUs);
            return true;
        }

        return false;
    }
}