using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSnakeLadderNoTurntableController:UIWindowController
{
    private Button CloseBtn;
    private Button StartBtn;
    private LocalizeTextMeshProUGUI PriceText;
    private LocalizeTextMeshProUGUI GreyPriceText;
    private LocalizeTextMeshProUGUI TurntableCountText;
    private LocalizeTextMeshProUGUI BuyTimesText;
    private StorageSnakeLadder Storage;
    private Transform BottomGroup;
    private Button EnterMergeBtn;
    private LocalizeTextMeshProUGUI TimeText;
    private int BuyTimes => Storage.GetCurDatBuyTimes();
    private int MaxBuyTimes => SnakeLadderModel.Instance.BuyTurntableConfig.Last().SaleTimes;
    SnakeLadderBuyTurntableConfig BuyConfig
    {
        get
        {
            var configs = SnakeLadderModel.Instance.BuyTurntableConfig;
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
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UISnakeLadderMainController>(UINameConst.UISnakeLadderMain);
            if (mainUI)
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
    }

    private int LastDayId;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageSnakeLadder;
        UpdateViewState();
        LastDayId = SnakeLadderUtils.CurDay;
        InvokeRepeating("UpdateTime",0f,1f);
        if (StartBtn.transform.TryGetComponent<ShieldButtonOnClick>(out var shield))
        {
            shield.isUse = false;
        }
    }

    public void UpdateTime()
    {
        TimeText.SetText(SnakeLadderUtils.CurDayLeftTimeString);
        if (LastDayId != SnakeLadderUtils.CurDay)
        {
            LastDayId = SnakeLadderUtils.CurDay;
            UpdateViewState();
        }
    }

    public void OnClickBuyBtn()
    {
        var buyConfig = BuyConfig;
        if (BuyTimes < MaxBuyTimes && UserData.Instance.CanAford(UserData.ResourceId.Diamond, buyConfig.Price))
        {
            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond,buyConfig.Price,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.SnakeLadderUse));
            Storage.AddCurDatBuyTimes(1);
            SnakeLadderModel.Instance.AddTurntable(buyConfig.TurntableCount,"QuickBuy");
            AnimCloseWindow();
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
            BottomGroup.gameObject.SetActive(false);
            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, buyConfig.Price))
            {
                StartBtn.interactable = true;
            }
            else
            {
                StartBtn.interactable = false;   
            }
        }
    }
    public static UIPopupSnakeLadderNoTurntableController Open(StorageSnakeLadder storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupSnakeLadderNoTurntable,storage) as
            UIPopupSnakeLadderNoTurntableController;
    }
}