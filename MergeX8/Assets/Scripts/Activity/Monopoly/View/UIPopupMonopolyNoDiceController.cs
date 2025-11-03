using System;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UIPopupMonopolyNoDiceController:UIWindowController
{
    private Button CloseBtn;
    private Button StartBtn;
    private LocalizeTextMeshProUGUI PriceText;
    private LocalizeTextMeshProUGUI GreyPriceText;
    private LocalizeTextMeshProUGUI TurntableCountText;
    private LocalizeTextMeshProUGUI BuyTimesText;
    private StorageMonopoly Storage;
    private Transform BottomGroup;
    private Button EnterMergeBtn;
    private LocalizeTextMeshProUGUI TimeText;
    private LocalizeTextMeshProUGUI DiceCountText;
    private int BuyTimes => Storage.GetCurDatBuyTimes();
    private int MaxBuyTimes => MonopolyModel.Instance.BuyDiceConfig.Last().SaleTimes;
    MonopolyBuyDiceConfig BuyConfig
    {
        get
        {
            var configs = MonopolyModel.Instance.BuyDiceConfig;
            for (var i = 0; i < configs.Count; i++)
            {
                if (configs[i].SaleTimes > BuyTimes)
                {
                    return configs[i];
                }
            }
            return configs.Last();
        }
    }
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(()=>AnimCloseWindow());
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(OnClickBuyBtn);
        PriceText = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text");
        GreyPriceText = GetItem<LocalizeTextMeshProUGUI>("Root/Button/GreyText");
        TurntableCountText = GetItem<LocalizeTextMeshProUGUI>("Root/Item/Text");
        BuyTimesText = GetItem<LocalizeTextMeshProUGUI>("Root/TextGroup/NumText");
        BottomGroup = GetItem<Transform>("Root/Bottom");
        EnterMergeBtn = GetItem<Button>("Root/Bottom/Button");
        EnterMergeBtn.onClick.AddListener(() =>
        {
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIMonopolyMainController>(UINameConst.UIMonopolyMain);
            if (mainUI && !mainUI.isPlaying)
                mainUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        });
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        DiceCountText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        EventDispatcher.Instance.AddEvent<EventMonopolyDiceCountChange>(OnDiceCountChange);
    }

    private void OnDestroy()
    {
        CurrencyGroupManager.Instance?.currencyController?.RecoverDiamondCanvasSortOrder();
        EventDispatcher.Instance.RemoveEvent<EventMonopolyDiceCountChange>(OnDiceCountChange);
    }

    public void OnDiceCountChange(EventMonopolyDiceCountChange evt)
    {
        // DiceCountText.SetText(Storage.DiceCount.ToString());
    }
    private int LastDayId;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageMonopoly;
        UpdateViewState();
        DiceCountText.SetText(Storage.DiceCount.ToString());
        LastDayId = MonopolyUtils.CurDay;
        InvokeRepeating("UpdateTime",0f,1f);
        if (StartBtn.transform.TryGetComponent<ShieldButtonOnClick>(out var shield))
        {
            shield.isUse = false;
        }
        CurrencyGroupManager.Instance?.currencyController?.SetDiamondCanvasSortOrder(canvas.sortingOrder + 1);
    }

    public void UpdateTime()
    {
        TimeText.SetText(MonopolyUtils.CurDayLeftTimeString);
        if (LastDayId != MonopolyUtils.CurDay)
        {
            LastDayId = MonopolyUtils.CurDay;
            UpdateViewState();
        }
    }

    public void OnClickBuyBtn()
    {
        var buyConfig = BuyConfig;
        if (BuyTimes < MaxBuyTimes)
        {
            var price = buyConfig.Price;
            var reason =
                new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyUse);
            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, price, out var needCount))
            {
                UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond,price,reason);
                Storage.AddCurDatBuyTimes(1);
                MonopolyModel.Instance.AddDice(buyConfig.TurntableCount,"QuickBuy");
                UpdateViewState();
                var count = Storage.DiceCount;
                FlyGameObjectManager.Instance.PerformFly((int)UserData.ResourceId.MonopolyDice,buyConfig.TurntableCount,
                    StartBtn.transform.position,DiceCountText.transform.position, () =>
                    {
                        DiceCountText.SetText(count.ToString());
                    });
                // AnimCloseWindow();   
            }
            else
            {
                BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "MonopolyQuickBuyDice",
                    "", "MonopolyQuickBuyDice",true,price);
            }
            // UpdateViewState();
        }
    }

    public void UpdateViewState()
    {
        var buyConfig = BuyConfig;
        PriceText.SetText(buyConfig.Price.ToString());
        GreyPriceText.SetText(buyConfig.Price.ToString());
        BuyTimesText.SetText(BuyTimes + "/"+MaxBuyTimes);
        TurntableCountText.SetText("x"+buyConfig.TurntableCount);
        if (BuyTimes >= MaxBuyTimes)
        {
            StartBtn.interactable = false;
            BottomGroup.gameObject.SetActive(true);
            
        }
        else
        {
            BottomGroup.gameObject.SetActive(true);
            StartBtn.interactable = true;
            if (UserData.Instance.CanAford(UserData.ResourceId.Coin, buyConfig.Price,out var needCount))
            {
            }
            else
            {
            }
        }
    }
    public static UIPopupMonopolyNoDiceController Open(StorageMonopoly storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupMonopolyNoDice,storage) as
            UIPopupMonopolyNoDiceController;
    }
}