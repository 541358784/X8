using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI;
using SomeWhere;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using Random = UnityEngine.Random;

public class UIPopupExchangeEnergyController : UIWindow
{
    public static string ExChangeEnergyKey = "ExChangeEnergyKey";
    
    private LocalizeTextMeshProUGUI titelText;
    private LocalizeTextMeshProUGUI diamondsNumText;
    private LocalizeTextMeshProUGUI countDownNumText;

    private Animator _animator;
    private int _countDownNum = 0;
    private Dictionary<int, Transform> _numTrans=new Dictionary<int, Transform>();
    public override void PrivateAwake()
    {
        BindClick("Root/ContentGroup/BGGroup/CloseButton", (go) =>
        {
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventExchangeEnergyClose);
            OnCloseClick(true);
        });
        Transform contentGroup = this.transform.Find("Root/ContentGroup");
        titelText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("TitleText");
        _numTrans.Add(500,contentGroup.Find("Num/500"));
        _numTrans.Add(800,contentGroup.Find("Num/800"));
        _numTrans.Add(1000,contentGroup.Find("Num/1000"));
        diamondsNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/BuyButton/Text");
        countDownNumText= contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("TimeGroup/TimeText");
        
        _animator = gameObject.GetComponent<Animator>();
        _countDownNum = GlobalConfigManager.Instance._exchangeEnergy.countDown*60;
        
        InvokeRepeating("CountDownRepeating", 0, 1);
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        
        
    }

    void Start()
    {
        int exChangeNum = 0;
        if (!StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData.ContainsKey(ExChangeEnergyKey))
            StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData[ExChangeEnergyKey] = 0;

        exChangeNum = StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData[ExChangeEnergyKey];

        exChangeNum = Math.Min(exChangeNum, GlobalConfigManager.Instance._exchangeEnergy.consumeNums.Length - 1);
        exChangeNum = Math.Max(exChangeNum, 0);
        
        int gemCount = GlobalConfigManager.Instance._exchangeEnergy.consumeNums[exChangeNum];
        int energyCount = GlobalConfigManager.Instance._exchangeEnergy.exchangeNums[exChangeNum];
        
        
        titelText.SetTerm(LocalizationManager.Instance.GetLocalizedString("&key.UI_common_need_help"));
        foreach (var num in _numTrans.Keys)
        {
            _numTrans[num].gameObject.SetActive(num==energyCount);
        }
        diamondsNumText.SetTerm(gemCount.ToString());

        BindClick("Root/ContentGroup/FG/BuyButton", (gameObject) =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.FreeEnergy);
            int needCount = gemCount;
            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, gemCount))
            {
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventExchangeEnergyGem);
                UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, gemCount,
                    new GameBIManager.ItemChangeReasonArgs()
                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ExchangeBuyEnergy});
                EnergyModel.Instance.AddEnergy(energyCount,
                    new GameBIManager.ItemChangeReasonArgs()
                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ExchangeBuyEnergy}, false, false);
                OnCloseClick();
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    UserData.ResourceId.Energy, energyCount, transform.position, 0.8f, true, true, 0.15f);

                StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData[ExChangeEnergyKey]++;
                exChangeNum = StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData[ExChangeEnergyKey];
                StorageManager.Instance.GetStorage<StorageHome>().StatisticsIntData[ExChangeEnergyKey] = Math.Min(exChangeNum, GlobalConfigManager.Instance._exchangeEnergy.consumeNums.Length - 1);
            }
            else
            {
                BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "ExChangeEnergy",
                    "", "ExChangeEnergy",true,needCount);
            }
        });
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
            }
        }));
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        OnCloseClick(true);
    }

    private void CountDownRepeating()
    {
        if(_countDownNum <= 0)
            return;
        
        _countDownNum--;
        countDownNumText.SetText(CommonUtils.FormatLongToTimeStr(_countDownNum*1000));
    }

    public static bool OpenUI()
    { 
        if(GiftBagSendOneModel.CanBuy())
            return false;
        
        if(EnergyPackageModel.Instance.IsCanShow())
            return false;

        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, ExChangeEnergyKey))
            return false;
        
        UIManager.Instance.OpenUI(UINameConst.UIPopupExchangeEnergy);
        return true;
    }
}