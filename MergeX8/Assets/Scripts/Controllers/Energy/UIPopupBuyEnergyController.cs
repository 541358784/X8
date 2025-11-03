using System;
using System.Collections.Generic;
using System.Linq;
using ABTest;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using ExtraEnergy;
using Gameplay;
using Gameplay.UI;
using SomeWhere;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using Random = UnityEngine.Random;

public class UIPopupBuyEnergyController : UIWindow
{
    private int LifeID = 110;
    private LocalizeTextMeshProUGUI titelText;
    private LocalizeTextMeshProUGUI introduceText;
    private LocalizeTextMeshProUGUI coinCoverNumText;
    private LocalizeTextMeshProUGUI rvCoverNumText;
    private LocalizeTextMeshProUGUI diamondsNumText;

    private Button rvBtn;
    private LocalizeTextMeshProUGUI rvText;
    private DailyShop _dailyShop;
    private Animator _animator;
    private string biData1;
    Dictionary<string, string> extras = new Dictionary<string, string>();
    private Transform gemGroup;
    private Transform rvGroup;
    public Transform ExtraViewHangPoint;
    // private DailyPackageExtraView DailyPackageExtraView;
    // private BPExtraView BPExtraView;
    private List<UIPopupExtraView> ExtraViewList;
    private LocalizeTextMeshProUGUI _addText;
    private LocalizeTextMeshProUGUI _addText2;
    public override void PrivateAwake()
    {
        BindClick("Root/ContentGroup/BGGroup/CloseButton", (go) =>
        {
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLackEnergyClose);
            OnCloseClick(true);
        });
        rvBtn = this.transform.GetComponent<Button>("Root/ContentGroup/FG1/WatchButton");
        rvBtn.transform.parent.gameObject.SetActive(
            !AdSubSystem.Instance.IsFailedByRvConfiged(ADConstDefine.RV_GET_ENERGY)&&!ABTestManager.Instance.IsOpenADTest());

        Transform contentGroup = this.transform.Find("Root/ContentGroup");
        gemGroup = this.transform.Find("Root/ContentGroup/FG");
        rvGroup = this.transform.Find("Root/ContentGroup/FG1");
        rvGroup.gameObject.SetActive(AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_GET_ENERGY));
        titelText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("TitleText");
        introduceText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("IntroduceText");
        coinCoverNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/NumText");
        _addText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/AddText");
        _addText2 = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/AddText/TagGroupEnergy/Text1");
        rvCoverNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG1/NumText");
        diamondsNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/BuyButton/Text");
        _animator = gameObject.GetComponent<Animator>();
        ExtraViewHangPoint = transform.Find("Root/ExtraViewHangPoint");
        ExtraViewList = new List<UIPopupExtraView>()
        {
            ExtraViewHangPoint.Find("BuyEnergyDailyPack").gameObject.AddComponent<NewDailyPackExtraView>(),
            ExtraViewHangPoint.Find("BuyEnergyBattlePass").gameObject.AddComponent<Activity.BattlePass.BattlePassExtraView>(),
            ExtraViewHangPoint.Find("BuyEnergyBattlePass2").gameObject.AddComponent<Activity.BattlePass_2.BattlePassExtraView>(),
            ExtraViewHangPoint.Find("BuyEnergyIcebreakingPack").gameObject.AddComponent<IcebreakingPackExtraView>(),
            ExtraViewHangPoint.Find("BuyEnergyIcebreakingPackLow").gameObject.AddComponent<IcebreakingPackLowExtraView>(),
        };
        // DailyPackageExtraView = ExtraViewHangPoint.Find("DailyPack").gameObject.AddComponent<DailyPackageExtraView>();
        // DailyPackageExtraView.InitUI();
        // BPExtraView = ExtraViewHangPoint.Find("BP").gameObject.AddComponent<BPExtraView>();
        // BPExtraView.InitUI();
        RefreshExtraView();
        InvokeRepeating("RefreshTime",0,1);
    }

    public void RefreshTime()
    {
        _addText.gameObject.SetActive(ExtraEnergyModel.Instance.IsOpened());
    }

    public void RefreshExtraView()
    {
        var tempCanShowExtraView = new List<UIPopupExtraView>();
        for (var i = 0; i < ExtraViewList.Count; i++)
        {
            if (ExtraViewList[i].CanShow())
                tempCanShowExtraView.Add(ExtraViewList[i]);
            else
                ExtraViewList[i].gameObject.SetActive(false);
        }

        if (tempCanShowExtraView.Count == 0)
        {
            ExtraViewHangPoint.gameObject.SetActive(false);
            return;
        }
        ExtraViewHangPoint.gameObject.SetActive(true);
        var showIndex = Random.Range(0, tempCanShowExtraView.Count);
        for (var i = 0; i < tempCanShowExtraView.Count(); i++)
        {
            tempCanShowExtraView[i].gameObject.SetActive(i == showIndex);
            if (i == showIndex)
            {
                tempCanShowExtraView[i].Init();
                tempCanShowExtraView[i].OnViewCanNotShow(RefreshExtraView);
            }
        }
        
        // var CanShowDailyPackageExtraView = DailyPackageExtraView.CanShow();
        // var CanShowBPExtraView = BPExtraView.CanShow();
        // if (!CanShowBPExtraView && !CanShowDailyPackageExtraView)
        // {
        //     ExtraViewHangPoint.gameObject.SetActive(false);
        //     return;
        // }
        // ExtraViewHangPoint.gameObject.SetActive(true);
        // var showDailyPackage = false;
        // if (CanShowDailyPackageExtraView && CanShowBPExtraView)
        //     showDailyPackage = Random.Range(0, 2) == 0;
        // else if (CanShowDailyPackageExtraView)
        //     showDailyPackage = true;
        // DailyPackageExtraView.gameObject.SetActive(showDailyPackage);
        // BPExtraView.gameObject.SetActive(!showDailyPackage);
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs != null && objs.Length > 0)
        {
            biData1 = (string) objs[0];
     
        }
        
        if (UserData.Instance.GetRes(UserData.ResourceId.Energy) <= 0 && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.FreeEnergy))
        {
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.AddEnergy, "0");
        }
    }

    void Start()
    {
        var rvAd = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, ADConstDefine.RV_GET_ENERGY);
        var bs = AdConfigHandle.Instance.GetBonus(rvAd.Bonus);
        int rvGetCount = bs == null && bs.Count <= 0 ? 10 : bs[0].count;
        UIAdRewardButton.Create(ADConstDefine.RV_GET_ENERGY, UIAdRewardButton.ButtonStyle.Disable, rvBtn.gameObject,
            (s) =>
            {
                try
                {
                    if (AdSubSystem.Instance.GetNeedPlayCount(ADConstDefine.RV_GET_ENERGY) -
                        AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_ENERGY) > 0 &&
                        AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_ENERGY) > 0)
                        rvText?.SetTerm(AdSubSystem.Instance.GetRvText(ADConstDefine.RV_GET_ENERGY));
                    GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLackEnergyRv);
                }
                catch (Exception e)
                {
                }
                // EnergyModel.Instance.AddEnergy(rvGetCount, new GameBIManager.ItemChangeReasonArgs() { reason=BiEventAdventureIslandMerge.Types.ItemChangeReason.AdEnergy},false,false);
                // FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(), UserData.ResourceId.Energy, rvGetCount, transform.position, 0.8f, true, true, 0.15f);
            }, false, null, () =>
            {
                if (AdSubSystem.Instance.GetNeedPlayCount(ADConstDefine.RV_GET_ENERGY) -
                    AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_ENERGY) <= 1)
                    OnCloseClick();
            });

        rvText = transform.Find("Root/ContentGroup/FG1/WatchButton/Text").GetComponent<LocalizeTextMeshProUGUI>();

        rvText.SetTerm(AdSubSystem.Instance.GetRvText(ADConstDefine.RV_GET_ENERGY));
        rvCoverNumText.SetText(rvGetCount.ToString());
        _dailyShop = AdConfigHandle.Instance.GetDailyShops().Find(x => x.Type == 4);
        var shopConfig = GlobalConfigManager.Instance.GetTableShopByID(_dailyShop.ShopItemId);
        var storageItem = StoreModel.Instance.GetStorageItem(shopConfig.id);
        int gemPrice = _dailyShop.Price[1] + storageItem.PriceAdd;
        int gemGetCount = shopConfig.amount;
        if (ExtraEnergyModel.Instance.IsOpened())
        {
            _addText.SetText("+"+ExtraEnergyModel.Instance.GetExtraEnergyActivityConfigByCount(storageItem.BuyCount).ExtraEnergy);
            _addText2.SetText(ExtraEnergyModel.Instance.GetExtraEnergyActivityConfigByCount(storageItem.BuyCount).ExtraEnergy+"%");
        }
        titelText.SetTerm(LocalizationManager.Instance.GetLocalizedString("&key.UI_common_need_help"));
        coinCoverNumText.SetText(gemGetCount.ToString());
        
        var buyButton = transform.Find("Root/ContentGroup/FG/BuyButton");
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(buyButton.transform);
        //topLayer.Add(CurrencyGroupManager.Instance.currencyController.transform.Find("Root/ResourcesGroup/CurrencyBarGroup1"));
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.FreeEnergy, buyButton as RectTransform, topLayer:topLayer);
        
        BindClick("Root/ContentGroup/FG/BuyButton", (gameObject) =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.FreeEnergy);
            int needCount = gemPrice;
            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, gemPrice))
            {
                if (gemPrice >= GlobalConfigManager.Instance._exchangeEnergy.consumeDiamond)
                {
                    if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, UIPopupExchangeEnergyController.ExChangeEnergyKey))
                        StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData[UIPopupExchangeEnergyController.ExChangeEnergyKey] = 0;
                    
                    CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, UIPopupExchangeEnergyController.ExChangeEnergyKey, CommonUtils.GetTimeStamp());
                }
                
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLackEnergyGem);
                UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, gemPrice,
                    new GameBIManager.ItemChangeReasonArgs()
                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy});
                if (ExtraEnergyModel.Instance.IsOpened())
                {
                    gemGetCount += ExtraEnergyModel.Instance.GetExtraEnergyActivityConfigByCount(storageItem.BuyCount).ExtraEnergy;
                }
                UserData.Instance.AddRes((int)UserData.ResourceId.Energy,gemGetCount,new GameBIManager.ItemChangeReasonArgs()
                    {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy});
                if (gemPrice > 0)
                {
                    EventDispatcher.Instance.SendEventImmediately(new EventBuyEnergy());
                }
               
                //if(gemPrice > 0)
                StoreModel.Instance.AddItemCount(shopConfig.id, _dailyShop);
                OnCloseClick();
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    UserData.ResourceId.Energy, gemGetCount, transform.position, 0.8f, true, true, 0.15f);
            }
            else
            {
                BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                    _dailyShop.ItemId.ToString(), "diamond_lack_flash",true,needCount);
            }
        });

        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.FreeEnergy, UserData.Instance.GetRes(UserData.ResourceId.Energy).ToString()))
            gemPrice = 0;
        diamondsNumText.SetTerm(gemPrice.ToString());
    }

    private void OnCloseClick(bool isJumpStore = false)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            bool isShouEnergyPackage = false;
            if (isJumpStore)
            {
                isShouEnergyPackage= EnergyPackageModel.Instance.CanShow();
                
                if (!isShouEnergyPackage)
                    isShouEnergyPackage = UIPopupExchangeEnergyController.OpenUI();
                if (ExperenceModel.Instance.GetLevel() < 4)
                {
                    MergeTaskTipsController.Instance._mergeDailyTaskItem?.GuideArrow2?.ShowForTime(15f);
                    MergeTaskTipsController.Instance._mergeDailyTaskItem?.GuideArrow2?.SetSortingOrder(MergeMainController.Instance.canvas.sortingOrder+1);
                }
                else if (ExperenceModel.Instance.GetLevel() == 4)
                {
                    if (!StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.ContainsKey("showArrow4"))
                    {
                        StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.Add("showArrow4", 1);
                        MergeMainController.Instance?.BackBtnGuideArrow?.ShowForTime(5f);
                        MergeMainController.Instance?.BackBtnGuideArrow?.SetSortingOrder(MergeMainController.Instance.canvas.sortingOrder+1);
                    }
                }
                else
                {
                    if (!StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.ContainsKey("showArrow5"))
                    {
                        StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.Add("showArrow5", 1);
                        MergeMainController.Instance?.BackBtnGuideArrow?.ShowForTime(5f);
                        MergeMainController.Instance?.BackBtnGuideArrow?.SetSortingOrder(MergeMainController.Instance.canvas.sortingOrder+1);
                    }
                }
            }

            if(!isShouEnergyPackage)
                GetMergeResource();
        }));
    }

    private void GetMergeResource()
    {
        if (UserData.Instance.GetRes(UserData.ResourceId.Energy) > 0)
            return;

        // if(SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
        //     MergeResourceManager.Instance.GetMergeResource(10201, MergeBoardEnum.Main,true);
    }


    public override void ClickUIMask()
    {
        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.FreeEnergy))
            return;
        
        if (!canClickMask)
            return;

        OnCloseClick(true);
    }

    public static void OpenUI(string src)
    { 
        UIManager.Instance.OpenUI(UINameConst.UIPopupBuyEnergy, src);
        
        // if (AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_GET_ENERGY) ||
        //     !UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GameStore))
        // {
        //     UIManager.Instance.OpenUI(UINameConst.UIPopupBuyEnergy, src);
        // }
        // else
        // {
        //     UIStoreController.OpenUI("energy_lack");
        // }
    }
}