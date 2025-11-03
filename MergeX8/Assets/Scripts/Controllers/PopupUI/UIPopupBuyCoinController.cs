using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using SomeWhere;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupBuyCoinController : UIWindow
{
    private int LifeID = 110;
    private LocalizeTextMeshProUGUI rvCoverNumText;
    private Button rvBtn;
    private LocalizeTextMeshProUGUI rvText;

    private LocalizeTextMeshProUGUI gemCoverNumText;
    private Button gemBtn;
    private LocalizeTextMeshProUGUI gemText;
    private DailyShop _dailyShop;
    private Animator _animator;
    private string biData1;
    private string biData3;
    private Transform gemGroup;
    Dictionary<string, string> extras = new Dictionary<string, string>();

    public override void PrivateAwake()
    {
        BindClick("Root/BGGroup/CloseButton", (go) =>
        {
            OnCloseClick(true);
        });

        rvBtn = this.transform.GetComponent<Button>("Root/ContentGroup/FG1/WatchButton");
        Transform contentGroup = this.transform.Find("Root/ContentGroup");
        rvCoverNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG1/NumText");
        rvText = transform.Find("Root/ContentGroup/FG1/WatchButton/Text").GetComponent<LocalizeTextMeshProUGUI>();
        gemGroup = this.transform.Find("Root/ContentGroup/FG");
        gemGroup.gameObject.SetActive(true);
        //gemGroup.gameObject.SetActive(!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GameStore));

        gemCoverNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/NumText");

        gemText = transform.Find("Root/ContentGroup/FG/BuyButton/Text").GetComponent<LocalizeTextMeshProUGUI>();

        _animator = gameObject.GetComponent<Animator>();

        _dailyShop = AdConfigHandle.Instance.GetDailyShops().Find(x => x.Type == 1 && x.Sold == 1);
        var shopConfig = GlobalConfigManager.Instance.GetTableShopByID(_dailyShop.ShopItemId);
        var storageItem = StoreModel.Instance.GetStorageItem(shopConfig.id);
        int gemPrice = _dailyShop.Price[1] + storageItem.PriceAdd;
        int gemGetCount = shopConfig.amount;
        gemCoverNumText.SetText("+" + gemGetCount);
        BindClick("Root/ContentGroup/FG/BuyButton", (gameObject) =>
        {
            int needCount = gemPrice;
            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, gemPrice))
            {
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLackEnergyGem);
                UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, gemPrice,
                    new GameBIManager.ItemChangeReasonArgs()
                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug});
                var reason = new GameBIManager.ItemChangeReasonArgs()
                    {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy};
                UserData.Instance.AddRes((int) UserData.ResourceId.Coin, gemGetCount, reason, false);
                StoreModel.Instance.AddItemCount(shopConfig.id, _dailyShop);
                OnCloseClick();
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    UserData.ResourceId.Coin, gemGetCount, transform.position, 0.8f, true, true, 0.15f);
            }
            else
            {
                BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                    _dailyShop.ItemId.ToString(), "diamond_lack_flash",true,needCount);
            }
        });
        gemText.SetTerm(gemPrice.ToString());
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs != null && objs.Length > 0)
        {
            biData1 = (string) objs[0];
            biData3 = (string) objs[1];
        }
    }

    void Start()
    {
        var rvAd = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, ADConstDefine.RV_GET_COIN);
        var bs = AdConfigHandle.Instance.GetBonus(rvAd.Bonus);
        int rvGetCount = bs == null && bs.Count <= 0 ? 10 : bs[0].count;
        UIAdRewardButton.Create(ADConstDefine.RV_GET_COIN, UIAdRewardButton.ButtonStyle.Disable, rvBtn.gameObject,
            (s) =>
            {
                if (AdSubSystem.Instance.GetNeedPlayCount(ADConstDefine.RV_GET_COIN) -
                    AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_COIN) > 0 &&
                    AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_COIN) > 0)
                    rvText?.SetTerm(AdSubSystem.Instance.GetRvText(ADConstDefine.RV_GET_COIN));
            }, false, null, () =>
            {
                if (AdSubSystem.Instance.GetNeedPlayCount(ADConstDefine.RV_GET_COIN) -
                    AdSubSystem.Instance.GetCurrentPlayCount(ADConstDefine.RV_GET_COIN) <= 1)
                    OnCloseClick();
            });

        rvText.SetTerm(AdSubSystem.Instance.GetRvText(ADConstDefine.RV_GET_COIN));
        rvCoverNumText.SetText("+" + rvGetCount);
    }

    private void OnCloseClick(bool isJumpStore = false)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            // if (isJumpStore && UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GameStore))
            // {
            //     UIStoreGameController.OpenUI("coin_lack");
            // }
        }));
    }

    public static bool TryShow(string src, string data3)
    {
        if (AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_GET_COIN) ||
            !UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GameStore))
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupBuyCoin, src, data3);
            return true;
        }

        return false;
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnCloseClick(false);
    }
}