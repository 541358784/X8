using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Gameplay.UI;
using SomeWhere;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using Random = UnityEngine.Random;

public class UIPopupHappyGoBuyEnergyController : UIWindow
{
    private int LifeID = 110;
    private LocalizeTextMeshProUGUI titelText;
    private LocalizeTextMeshProUGUI introduceText;
    private LocalizeTextMeshProUGUI coinCoverNumText;
    private LocalizeTextMeshProUGUI rvCoverNumText;
    private LocalizeTextMeshProUGUI diamondsNumText;

    private DailyShop _dailyShop;
    private Animator _animator;
    private string biData1;
    Dictionary<string, string> extras = new Dictionary<string, string>();
    private Transform gemGroup;
    public override void PrivateAwake()
    {
        BindClick("Root/ContentGroup/BGGroup/CloseButton", (go) =>
        {
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLackEnergyClose);
            OnCloseClick(true);
        });
    

        Transform contentGroup = this.transform.Find("Root/ContentGroup");
        gemGroup = this.transform.Find("Root/ContentGroup/FG");
        titelText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("TitleText");
        introduceText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("IntroduceText");
        coinCoverNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/NumText");
        diamondsNumText = contentGroup.GetComponentDefault<LocalizeTextMeshProUGUI>("FG/BuyButton/Text");
        _animator = gameObject.GetComponent<Animator>();
    
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs != null && objs.Length > 0)
        {
            biData1 = (string) objs[0];
     
        }
        
    }

    void Start()
    {
        _dailyShop = AdConfigHandle.Instance.GetDailyShops().Find(x => x.Type == 5);
        var shopConfig = GlobalConfigManager.Instance.GetTableShopByID(_dailyShop.ShopItemId);
        var storageItem = StoreModel.Instance.GetStorageItem(shopConfig.id);
        int gemPrice = _dailyShop.Price[1] + storageItem.PriceAdd;
        int gemGetCount = shopConfig.amount;
        titelText.SetTerm(LocalizationManager.Instance.GetLocalizedString("&key.UI_common_need_help"));
        coinCoverNumText.SetText(gemGetCount.ToString());
    
        BindClick("Root/ContentGroup/FG/BuyButton", (gameObject) =>
        {
            int needCount = gemPrice;
            if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, gemPrice))
            {
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLackEnergyGem);
                UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, gemPrice,
                    new GameBIManager.ItemChangeReasonArgs()
                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy});
                HappyGoEnergyModel.Instance.AddEnergy(gemGetCount,
                    new GameBIManager.ItemChangeReasonArgs()
                        {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ShopBuyEnergy}, false, false);
                //if(gemPrice > 0)
                StoreModel.Instance.AddItemCount(shopConfig.id, _dailyShop);
                AnimCloseWindow();
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    UserData.ResourceId.HappyGo_Energy, gemGetCount, transform.position, 0.8f, true, true, 0.15f);

            }
            else
            {
                BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                    _dailyShop.ItemId.ToString(), "diamond_lack_flash",true,needCount);
            }
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgVdBuyenergyPanel);
        });

        diamondsNumText.SetTerm(gemPrice.ToString());
    }

    private void OnCloseClick(bool isJumpStore = false)
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
            HappyGoPackModel.Instance.TryShowEnergyPack();
        }));
    }

    private void GetMergeResource()
    {
        if (UserData.Instance.GetRes(UserData.ResourceId.Energy) > 0)
            return;

        //MergeResourceManager.Instance.GetMergeResource(10201, MergeBoardEnum.Main,true);
    }


    public override void ClickUIMask()
    {
        
        if (!canClickMask)
            return;

        OnCloseClick(true);
    }

    public static void OpenUI(string src)
    { 
        UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoBuyEnergy, src);
        
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