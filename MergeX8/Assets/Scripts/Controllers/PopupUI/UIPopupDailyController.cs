using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupDailyController : UIWindowController
{
    private LocalizeTextMeshProUGUI _diamondRewardCountText ;
    // private Image _diamondRewardIcon ;   
    
    private LocalizeTextMeshProUGUI _energyRewardCountText ;
    // private Image _energyRewardIcon ;

    private bool isAdPlay = false;

    private static string constPlaceId = ADConstDefine.RV_DAILY_BONUS;

    private Animator _animator;

    private GameObject person_1;
    private GameObject person_2;

    private bool isChangePerson = false;

    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();
        _diamondRewardCountText = GetItem<LocalizeTextMeshProUGUI>("Root/WindowsGroup/MiddleGroup/Diamond/Text");
         _energyRewardCountText = GetItem<LocalizeTextMeshProUGUI>("Root/WindowsGroup/MiddleGroup/Energy/Text");

        person_1 = transform.Find("Root/WindowsGroup/MiddleGroup/Person1").gameObject;

        person_2 = transform.Find("Root/WindowsGroup/MiddleGroup/Person2").gameObject;
        person_2.SetActive(false);

        Button buttonAds = GetItem<Button>("Root/WindowsGroup/ButtonsGroup/ReplayButton");
        AdRewardedVideoPlacementMonitor.Bind(buttonAds.gameObject, constPlaceId);
        var rewardList = AdConfigHandle.Instance.GetBonus(constPlaceId);
        if (rewardList != null && rewardList.Count > 0)
        {
            if (rewardList[0].id == (int) UserData.ResourceId.Diamond)
            {
                _diamondRewardCountText.SetText(rewardList[0].count.ToString());
                _energyRewardCountText.transform.parent.gameObject.SetActive(false);
                _diamondRewardCountText.transform.parent.gameObject.SetActive(true);

            }else if (rewardList[0].id == (int) UserData.ResourceId.Energy)
            {
                _diamondRewardCountText.transform.parent.gameObject.SetActive(false);
                _energyRewardCountText.transform.parent.gameObject.SetActive(true);
                _energyRewardCountText.SetText(rewardList[0].count.ToString());
            }
        
        }

        buttonAds.onClick.AddListener(() =>
        {
            if (isAdPlay)
                return;

            isAdPlay = true;
            StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
                () => { CloseWindowWithinUIMgr(true); }));
#if UNITY_EDITOR
            var edrv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, constPlaceId);
            if (edrv != null && edrv.Bonus > 0)
            {
                var bs = AdConfigHandle.Instance.GetBonus(edrv.Bonus);
                if (bs == null || bs.Count == 0)
                {
                    DebugUtil.LogError("bonus is null " + UserGroupManager.Instance.SubUserGroup + "\t" + constPlaceId);
                }

                if (bs != null && bs.Count >= 1)
                {
                 
                    List<ResData> list = new List<ResData>();
                    ResData resData = new ResData(bs[0].id, bs[0].count);
                    list.Add(resData);
                    var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Adv);
                    reasonArgs.data2 = constPlaceId;
                    CommonRewardManager.Instance.PopCommonReward(list, CurrencyGroupManager.Instance.currencyController,
                        true, reasonArgs, () => { });
                }
            }
#else
            AdSubSystem.Instance.PlayRV(constPlaceId, PlayRvCallBack);
#endif
        });

        Button buttonClose = GetItem<Button>("Root/WindowsGroup/ButtonClose");
        buttonClose.onClick.AddListener(() => { ClickUIMask(); });


        List<ResData> resDatas = new List<ResData>();
        var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, constPlaceId);
        if (rv != null && rv.Bonus > 0)
        {
            var bs = AdConfigHandle.Instance.GetBonus(rv.Bonus);
            if (bs == null || bs.Count == 0)
            {
                DebugUtil.LogError("bonus is null " + UserGroupManager.Instance.SubUserGroup + "\t" + constPlaceId);
            }

            if (bs != null && bs.Count >= 1)
            {
                _diamondRewardCountText.SetText(bs[0].count.ToString());
                _energyRewardCountText.SetText(bs[0].count.ToString());
                ResData resData = new ResData(bs[0].id, bs[0].count);
                resDatas.Add(resData);
            }
        }

        Button buttonFree = GetItem<Button>("Root/WindowsGroup/ButtonsGroup/FreeButton");
        buttonFree.onClick.AddListener(() =>
        {
            CloseWindowWithinUIMgr(true);

            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Adv);
            reasonArgs.data2 = constPlaceId;
            reasonArgs.data1 = "Free";
            CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController,
                true, reasonArgs, () => { });
            AdSubSystem.Instance.ADPlayRecord(constPlaceId);
        });

        int rewardType = AdConfigHandle.Instance.GetDailyRewardType();
        buttonAds.gameObject.SetActive(rewardType == 0);
        buttonFree.gameObject.SetActive(rewardType != 0);
    }

    public static bool CanShowUI()
    {
        // List<DragonPlus.ConfigHub.Ad.Global> adGlobal = AdConfigManager.Instance.GetConfig<DragonPlus.ConfigHub.Ad.Global>();
        // if (adGlobal != null && adGlobal.Count > 0)
        // {
        //     // int passLevelCount = LevelGroupSystem.Instance.GetPassedLevelCount();
        //     // if (passLevelCount < adGlobal[0].Daily_Bonus_Level)
        //     //     return false;
        // }

        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
            return false;

        if (AdConfigHandle.Instance.GetDailyRewardType() == 0)
        {
            if (!AdSubSystem.Instance.CanPlayRV(constPlaceId))
                return false;
        }
        else
        {
            if (!AdSubSystem.Instance.CanPlayRVFilterFill(constPlaceId))
                return false;
        }

        ShowUI();
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,
            CommonUtils.GetTimeStamp());
        return true;
    }

    public static void ShowUI()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupDaily);
    }

    private void PlayRvCallBack(bool isSuccess)
    {
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        if (!isChangePerson)
        {
            isChangePerson = true;
            person_1.SetActive(false);
            person_2.SetActive(true);
            return;
        }

        canClickMask = false;


        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }
}