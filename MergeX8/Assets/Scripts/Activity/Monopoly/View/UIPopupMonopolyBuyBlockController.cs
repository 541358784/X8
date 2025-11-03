using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using IFix.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMonopolyBuyBlockController : UIWindowController
{
    private Button CloseBtn;
    private Button BuyButton;
    private LocalizeTextMeshProUGUI PriceText;
    private LocalizeTextMeshProUGUI TitleText;
    private StorageMonopoly Storage;
    private MonopolyBlockConfig BlockConfig;
    private Action Callback;
    private UIMonopolyMainController.BlockNode OldBlock;
    private UIMonopolyMainController.BlockNode NewBlock;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(Callback);
        });
        BuyButton = GetItem<Button>("Root/Button");
        BuyButton.onClick.AddListener(OnClickBuyButton);
        PriceText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        TitleText = GetItem<LocalizeTextMeshProUGUI>("Root/TitleText");
        OldBlock = transform.Find("Root/Icon1").gameObject.AddComponent<UIMonopolyMainController.BlockNode>();
        NewBlock = transform.Find("Root/Icon2").gameObject.AddComponent<UIMonopolyMainController.BlockNode>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageMonopoly;
        BlockConfig = objs[1] as MonopolyBlockConfig;
        Callback = objs[2] as Action;
        var price = Storage.GetBuyBlockPrice(BlockConfig);
        PriceText.SetText(price.ToString());
        TitleText.SetTermFormats(price.ToString());
        var oldBuyTimes = Storage.GetBlockBuyTimes(BlockConfig.Id);
        var newBuyTimes = oldBuyTimes +1;
        var oldFull = Storage.IsBlockGroupFull(BlockConfig);
        var newFull = Storage.IsBlockGroupFull(BlockConfig,true);
        OldBlock.Init(BlockConfig,oldBuyTimes,oldFull);
        NewBlock.Init(BlockConfig,newBuyTimes,newFull);

        var enough = UserData.Instance.CanAford(UserData.ResourceId.Coin, price,out var needCount);
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyBuyBlock))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(BuyButton.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MonopolyBuyBlock, BuyButton.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyBuyBlock, null))
            {
                
            }
        }
        CurrencyGroupManager.Instance?.currencyController?.SetCoinCanvasSortOrder(canvas.sortingOrder+1);
    }

    private void OnDestroy()
    {
        CurrencyGroupManager.Instance?.currencyController?.RecoverCoinCanvasSortOrder();
    }

    public void OnClickBuyButton()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyBuyBlock);
        var price = Storage.GetBuyBlockPrice(BlockConfig);
        var enough = UserData.Instance.CanAford(UserData.ResourceId.Coin, price,out var needCount);
        var reason = new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.MonopolyUse
        };
        if (!enough)
        {
            CurrencyGroupManager.Instance?.currencyController?.SetDiamondCanvasSortOrder(999);
            CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedStringWithFormats("UI_MonopolyActive_LackCoins_Title",
                    needCount.ToString(),needCount.ToString()),
                OKCallback = () =>
                {
                    CurrencyGroupManager.Instance?.currencyController?.RecoverDiamondCanvasSortOrder();
                    var diamondEnough = UserData.Instance.CanAford(UserData.ResourceId.Diamond, needCount,out var diamondNeedCount);
                    if (diamondEnough)
                    {
                        if (price > needCount)
                        {
                            UserData.Instance.ConsumeRes(UserData.ResourceId.Coin,price-needCount,reason);   
                        }
                        UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond,needCount,reason);
                        Storage.BuyBlock(BlockConfig);
                        AnimCloseWindow(Callback);
                    }
                    else
                    {
                        BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "MonopolyBuyBlock"+BlockConfig.Id,
                            "", "MonopolyBuyBlock",true,needCount);
                    }
                },
                HasCloseButton = true,
                HasCancelButton = true,
                IsHighSortingOrder = true,
                CancelCallback = ()=>CurrencyGroupManager.Instance?.currencyController?.RecoverDiamondCanvasSortOrder(),
            });
        }
        else
        {
            UserData.Instance.ConsumeRes(UserData.ResourceId.Coin,price,reason);
            Storage.BuyBlock(BlockConfig);
            AnimCloseWindow(Callback);
        }
    }

    public static UIPopupMonopolyBuyBlockController Open(StorageMonopoly storage,MonopolyBlockConfig blockConfig,Action callback = null)
    {
        if (!storage.CanBuyBlock(blockConfig))
            return null;
        return UIManager.Instance.OpenUI(UINameConst.UIPopupMonopolyBuyBlock,storage,blockConfig,callback) as
            UIPopupMonopolyBuyBlockController;
    }
}