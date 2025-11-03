using System;
using DragonPlus;
using UnityEngine;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using SomeWhere;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIBuyResourcesController : UIWindow
{
    private LocalizeTextMeshProUGUI tileText;
    private LocalizeTextMeshProUGUI introduceText;
    private LocalizeTextMeshProUGUI buyPriceText;
    private LocalizeTextMeshProUGUI rewardNumText;
    private Animator _animator;

    private UserData.ResourceId buyResType = UserData.ResourceId.None;

    private Image iconImage;

    private GameObject buyButtonObj;

    private int shopId = -1;
    private int rewardNum = 0;
    private string iconName = "";
    private string source;
    private string data;
    private string storeSource;

    public override void PrivateAwake()
    {
        EventDispatcher.Instance.AddEventListener(EventEnum.BUYSOURCES_PURCHASE, BuySourcesPurchase);
        tileText = transform.Find("Root/ContentGroup/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        introduceText = transform.Find("Root/ContentGroup/IntroduceText").GetComponent<LocalizeTextMeshProUGUI>();
        rewardNumText = transform.Find("Root/ContentGroup/FG/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        buyPriceText = transform.Find("Root/ContentGroup/FG/BuyButton/Text").GetComponent<LocalizeTextMeshProUGUI>();

        buyButtonObj = transform.Find("Root/ContentGroup/FG/BuyButton").gameObject;

        iconImage = transform.Find("Root/ContentGroup/FG/Icon").GetComponent<Image>();

        BindClick("Root/BGGroup/CloseButton", (go) =>
        {
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventCoinClose, biData1,TaskModuleManager.Instance.GetCurMaxTaskID().ToString(),biData3);
            OnCloseClick(false);
        });

        _animator = gameObject.GetComponent<Animator>();

        BindClick("Root/ContentGroup/FG/BuyButton", (gameObject) =>
        {
            //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventEnergyGem, biData1,TaskModuleManager.Instance.GetCurMaxTaskID().ToString(),gemPrice.ToString());

            StoreModel.Instance.Purchase(shopId);
        });
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);

        if (objs == null || objs.Length == 0)
            return;

        buyResType = (UserData.ResourceId) (objs[0]);
        source = (string) objs[1];
        data = (string) objs[2];
        storeSource = (string) objs[3];
    }

    void Start()
    {
        string titleKey = "";
        string introduceKye = "";

        switch (buyResType)
        {
            case UserData.ResourceId.Coin:
            {
                titleKey = "UI_common_not_enough_coins";
                introduceKye = "UI_common_not_enough_coins_desc3";
                break;
            }
            case UserData.ResourceId.Diamond:
            {
                titleKey = "UI_common_not_enough_gems";
                introduceKye = "UI_common_not_enough_gems_desc3";
                break;
            }
            case UserData.ResourceId.Energy:
            {
                titleKey = "UI_common_energy";
                introduceKye = "UI_RefillLives_text";
                break;
            }
        }

        tileText.SetTerm(titleKey);
        introduceText.SetTerm(introduceKye);
        buyPriceText.SetText("0");
        rewardNumText.SetText("0");
        iconImage.sprite = null;

        if (BuyResourceManager.Instance.GetBuyResourceData(buyResType, out shopId, out rewardNum, out iconName))
        {
            rewardNumText.SetText(rewardNum.ToString());

            TableShop shopData = GlobalConfigManager.Instance.GetTableShopByID(shopId);
            if (shopData == null)
            {
                buyButtonObj.gameObject.SetActive(false);
                return;
            }

            iconImage.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, iconName);
            buyPriceText.SetText(StoreModel.Instance.GetPrice(shopId));
        }
        else
        {
            buyButtonObj.gameObject.SetActive(false);
        }
    }

    private void OnCloseClick(bool isBuy = false)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            BuyResourceManager.Instance.SetResCloseData(buyResType, !isBuy);

            if (!isBuy)
                BuyResourceManager.Instance.TryShowBuyResource(buyResType, source, data, storeSource, false);

            // if (isJumpStore && UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GameStore))
            // {
            //     UIStoreGameController.OpenUI("coin_lack");
            // }
        }));
    }

    public override void ClickUIMask()
    {
        OnCloseClick(false);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BUYSOURCES_PURCHASE, BuySourcesPurchase);
    }

    private void BuySourcesPurchase(BaseEvent e)
    {
        OnCloseClick(true);
    }
}