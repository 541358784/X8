using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupRVRewardController : UIWindowController
{
    private Button _buttonClose;
    private Button _buttonWatch;
    private GameObject _objAdLoading;
    private List<UIPopupRVRewardItem> _rewardItems;
    private LocalizeTextMeshProUGUI buttonLoadText;
    private LocalizeTextMeshProUGUI descText;
    private string coolTimeKey = "RvRewardCookTimeKey";
    private static string coolTimeLevelKey = "RvRewardCookTimeLevelKey";
    private static List<int> popUpUINum = new List<int>();


    private Image uiSliderImage = null;
    private GameObject contentObj = null;
    private int limitPerDay = 0;

    private List<GameObject> gameItem = new List<GameObject>();

    private int[] sliderLengthArray = {129, 108, 123};

    private int sliderLength = 0;
    //List<TableRvReward> rvRewardsTable = new List<TableRvReward>();

    public override void PrivateAwake()
    {
        CommonUtils.TweenOpen(GetItem("Root").transform);

        contentObj = GetItem("Root/MiddleGroup/Scroll View/Viewport/Content");

        uiSliderImage = GetItem<Image>("Root/MiddleGroup/Scroll View/Viewport/Content/UISlider");

        for (int i = 0; i <= 2; i++)
        {
            gameItem.Add(GetItem("Root/MiddleGroup/Scroll View/Viewport/Content/item" + i));
            gameItem[i].gameObject.SetActive(false);
        }

        _rewardItems = new List<UIPopupRVRewardItem>();

        GetItem<ScrollRect>("Root/MiddleGroup/Scroll View").onValueChanged.AddListener(arg0 =>
        {
            if (arg0.y <= 0 || arg0.y >= 1)
                return;

            foreach (var kv in _rewardItems)
            {
                kv.RestUI();
            }
        });

        //var rvRewards=  GlobalConfigManager.Instance.GetTableRvRewards();

        int rewardIndex = 0;
        int allLength = 0;

        int rvRewardId = RvRewardId;
        //var c = AdDataConfigManager.Instance.GetRVData((CommercialUserType)UserGroupManager.Instance.CurUserType, ConstDefine.RV_TVReward);
        // if (c != null)
        // {
        //     limitPerDay = c.limitPerDay;
        //     for (int i = 0; i < limitPerDay; i++)
        //     {
        //         if(rewardIndex >= rvRewards.Count)
        //             rewardIndex = 0;
        //         
        //         rvRewardsTable.Add(rvRewards[rewardIndex]);
        //
        //         rewardIndex++;
        //     }
        //
        //     rewardIndex = 0;
        //     sliderLength = 0;
        //     
        //     allLength = 0;
        //     for (int i = rvRewardsTable.Count-1; i >= 0; i--)
        //     {
        //         if(rewardIndex >= rvRewards.Count)
        //             rewardIndex = 0;
        //
        //         int type = rvRewardsTable[i].rewardType;
        //         GameObject item = gameItem[1];
        //         if (i == 0)
        //             item = gameItem[0];
        //         else if(type == 2)
        //             item = gameItem[2];
        //         
        //         GameObject rewardItem = Instantiate(item, contentObj.transform);
        //         rewardItem.gameObject.SetActive(true);
        //         var mono = rewardItem.gameObject.AddComponent<UIPopupRVRewardItem>();
        //         mono.SetData(rvRewardsTable[i], i, rvRewardId);
        //        
        //         _rewardItems.Add(mono);
        //
        //         rewardIndex++;
        //   
        //         if (i >= rvRewardId + 5)
        //         {
        //             allLength += rvRewardsTable[i].rewardType == 2 ? 130 : 100;
        //             allLength += 8;
        //         }
        //     }
        // }
        //
        // for (int i = 0; i < rvRewardsTable.Count; i++)
        // {
        //     if (i >= rvRewardId)
        //         break;
        //     
        //     if (rvRewardsTable[i].rewardType == 2 || i+1 <= rvRewardsTable.Count-1 &&  rvRewardsTable[i+1].rewardType == 2)
        //         sliderLength += sliderLengthArray[2];
        //     else if (i == 0)
        //         sliderLength += sliderLengthArray[0];
        //     else
        //         sliderLength += sliderLengthArray[1];
        // }


        allLength -= 50;
        AnchorPosY(contentObj.transform as RectTransform, allLength);

        uiSliderImage.rectTransform.SetHeight(sliderLength);

        _buttonClose = GetItem<Button>("Root/UIBg/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonWatch = GetItem<Button>("Root/ButtonGroup/ButtonWatch");
        _buttonWatch.onClick.AddListener(OnWatchRv);

        _objAdLoading = GetItem("Root/ButtonGroup/ButtonLoad");
        buttonLoadText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonGroup/ButtonLoad/Text");
        descText = GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/Text");
        buttonLoadText.SetText(LocalizationManager.Instance.GetLocalizedString("&key.UI_rv_prepare_button"));
        //AdRewardedVideoPlacementMonitor.Bind(_buttonWatch.gameObject, ConstDefine.RV_TVReward);
        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAdTvRewardPop);

        UpdateUI();
    }

    private void UpdateUI()
    {
        //bool canPlayRv = AdSubSystem.Instance.CanPlayRV(ConstDefine.RV_TVReward);
        // _objAdLoading.SetActive(!canPlayRv);
        // _buttonWatch.gameObject.SetActive(canPlayRv);
        //
        // if (!canPlayRv)
        // {
        //     InvokeRepeating("CheckRv", 2f, 1f);
        //     descText.SetText(LocalizationManager.Instance.GetLocalizedString("&key.UI_rv_prepare"));
        // }
    }

    private void OnBtnClose()
    {
        CloseWindowWithinUIMgr(true);
    }

    private int rvindex = 0;

    private void OnWatchRv()
    {
        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAdTvReward);

        // if (!AdSubSystem.Instance.CanPlayRV(ConstDefine.RV_TVReward))
        // {
        //     //Debug.LogError("RV 不能播放 1");
        //     return;
        // }
        // AdSubSystem.Instance.PlayRV(ConstDefine.RV_TVReward, b =>
        // {
        //     if(!b)
        //         return;
        //     
        //     int id = 1;
        //     if (rvRewardsTable.Count > RvRewardId)
        //         id = rvRewardsTable[RvRewardId].id;
        //     
        //     RvRewardId++;
        //     
        //     CommonUtils.ShowRewardUpdate(GetRvReward(id),
        //         new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.AdTvReward),
        //         () =>
        //         {
        //             EventDispatcher.Instance.DispatchEvent(EventEnum.UpdateRvReward);
        //
        //
        //             if (!CanGetRvReward())
        //             {
        //                 //Debug.LogError("RV 播放到上限了");
        //                 return;
        //             }
        //
        //             if (!AdSubSystem.Instance.CanPlayRV(ConstDefine.RV_TVReward))
        //             {
        //                 //Debug.LogError("RV 没有装载好");
        //                 return;
        //             }
        //             
        //             UIManager.Instance.OpenUI(UINameConst.UIPopupRVReward);
        //         });
        //         
        //     // if (RvRewardId >= limitPerDay)
        //     //     RvRewardId = 0;
        //     
        //     OnBtnClose();
        // },true );
    }

    private void CheckRv()
    {
        // bool canPlayRv = AdSubSystem.Instance.CanPlayRV(ConstDefine.RV_TVReward);
        // if(!canPlayRv)
        //     return;
        //
        // _objAdLoading.SetActive(!canPlayRv);
        // _buttonWatch.gameObject.SetActive(canPlayRv);
        //
        // descText.SetText(LocalizationManager.Instance.GetLocalizedString("&key.UI_rv_reward_desc"));
        // CancelInvoke("CheckRv");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
    }
    //
    // private TableRvReward GetRvRewardConfig(int id)
    // {
    //     foreach (var kv in  GlobalConfigManager.Instance.GetTableRvRewards())
    //     {
    //         if (kv.id == id)
    //             return kv;
    //     }
    //
    //     return null;
    // }
    //
    // private List<ResData> GetRvReward(int id)
    // {
    //     TableRvReward tableConfig = GetRvRewardConfig(id);
    //     if (tableConfig == null)
    //         return null;
    //
    //
    //     List<ResData> rewardList = new List<ResData>();
    //
    //     for (int i = 0; i < tableConfig.rewardId.Length; i++)
    //     {
    //         ResData resData = new ResData(tableConfig.rewardId[i], tableConfig.rewardNum[i]);
    //         rewardList.Add(resData);
    //     }
    //     return rewardList;
    //
    // }


    private int RvRewardId
    {
        get
        {
            if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            {
                StorageManager.Instance.GetStorage<StorageHome>().RvRewardId = 0;
            }

            return StorageManager.Instance.GetStorage<StorageHome>().RvRewardId;
        }
        set
        {
            StorageManager.Instance.GetStorage<StorageHome>().RvRewardId = value;
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
                CommonUtils.GetTimeStamp());
        }
    }

    private void OnDestroy()
    {
        CancelInvoke("CheckRv");
    }

    public static bool CanGetRvReward()
    {
        return false;
        //return !AdSubSystem.Instance.IsFailedByRVWithinLimit(ConstDefine.RV_TVReward);
    }

    public static void InitRvReward()
    {
        if (!CanGetRvReward())
            return;

        // if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeLevelKey))
        // {
        //     CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeLevelKey, CommonUtils.GetTimeStamp());
        //     StorageManager.Instance.GetStorage<StorageHome>().RvRewardLevel = GameCommonLogic.Instance.GetMaxRound();
        //     StorageManager.Instance.GetStorage<StorageHome>().ManualTouchRv = false;
        //     StorageManager.Instance.GetStorage<StorageHome>().RvRewardPopCount = 0;
        // }

        if (popUpUINum.Count == 0)
        {
            // var rv = AdDataConfigManager.Instance.GetRVData((CommercialUserType)UserGroupManager.Instance.CurUserType,  ConstDefine.RV_TVReward);
            // popUpUINum.AddRange(rv.adTime);
        }
    }

    public static bool CanShowUI()
    {
        if (!CanGetRvReward())
            return false;

        if (StorageManager.Instance.GetStorage<StorageHome>().ManualTouchRv)
            return false;

        // if (!AdSubSystem.Instance.CanPlayRV(ConstDefine.RV_TVReward))
        //     return false;


        // int level = GameCommonLogic.Instance.GetMaxRound() -
        //             StorageManager.Instance.GetStorage<StorageHome>().RvRewardLevel;
        //
        // if (popUpUINum.Count <2)
        //     return false;
        //
        //
        // if (level >= popUpUINum[0]&&StorageManager.Instance.GetStorage<StorageHome>().RvRewardPopCount<popUpUINum[1])
        // {
        //     StorageManager.Instance.GetStorage<StorageHome>().RvRewardPopCount++;
        //     StorageManager.Instance.GetStorage<StorageHome>().RvRewardLevel = GameCommonLogic.Instance.GetMaxRound();
        //     UIManager.Instance.OpenUI(UINameConst.UIPopupRVReward);
        //     return true;
        // }


        return false;
    }

    public RectTransform AnchorPosY(RectTransform selfRectTrans, float anchorPosY)
    {
        var anchorPos = selfRectTrans.anchoredPosition;
        anchorPos.y = anchorPosY;
        selfRectTrans.anchoredPosition = anchorPos;
        return selfRectTrans;
    }
}