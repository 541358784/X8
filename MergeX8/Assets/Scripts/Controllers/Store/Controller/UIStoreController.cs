using System;
using System.Collections;
using System.Collections.Generic;
using ActivityLocal.ClimbTower.Model;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay.UI.Store.Controller;
using Gameplay.UI.Store.Vip.Model;
using GamePool;
using Merge.Order;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIStoreController : UIWindow
{
    public enum ShopOpenType
    {
        Gem = 0,
        Coin = 1
    }

    public static UIStoreController Instance;
    private Button _btnClose;
    private Transform _tsContentGroup;
    private ScrollRect _scrollRect;
    private RectTransform _content;
    private Dictionary<int, UIStoreBaseItem> _dicItems = new Dictionary<int, UIStoreBaseItem>();
    private Dictionary<int, UIStoreTaskAssistBundleItem> _dicTaskAssistItems =
        new Dictionary<int, UIStoreTaskAssistBundleItem>();

    private List<UIStoreFlashItem> flashShopItems = new List<UIStoreFlashItem>();
    private List<UIStorePigSaleItem> pigShopItems = new List<UIStorePigSaleItem>();
    private Dictionary<ShowArea, GameGiftTitleItem> _areaTitles = new Dictionary<ShowArea, GameGiftTitleItem>();
    private VerticalLayoutGroup _layout;
    private ContentSizeFitter _csf;
    private SimpleGameObjectPool _pool;
    private Animator _animator;
    private Transform _giftTitle;
    private GameGiftTitleItem titleItem;
    private string openSrc = "";
    private ShowArea firstArea=ShowArea.none;
    
    private List<UIDailyBundleItem> _bundleItems;
    private List<UINewDailyBundleItem> _bundleItemsNew;

    
    public override void PrivateAwake()
    {
        Instance = this;

        CommonUtils.NotchAdapte(transform.Find("Root") as RectTransform);
        _animator = gameObject.GetComponent<Animator>();
        _btnClose = transform.Find("Root/LevelState2/CloseButton").GetComponent<Button>();
        _tsContentGroup = transform.Find("Root/LevelState2/Viewport/Content");
        _scrollRect = transform.Find("Root/LevelState2").GetComponent<ScrollRect>();
        _content = transform.Find("Root/LevelState2/Viewport/Content") as RectTransform;
        _layout = _tsContentGroup.GetComponent<VerticalLayoutGroup>();
        _csf = _tsContentGroup.GetComponent<ContentSizeFitter>();
        
        _giftTitle = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.GameGiftTitlePath).transform;
        _btnClose.onClick.AddListener(OnClickClose);
        _bundleItems = new List<UIDailyBundleItem>();
        _bundleItemsNew = new List<UINewDailyBundleItem>();
        EventDispatcher.Instance.AddEventListener(EventEnum.HOTEL_PURCHASE_SUCCESS, OnPurchase);
        EventDispatcher.Instance.AddEventListener(EventEnum.HOTEL_CLAIM_FREE_SUCCESS, OnClaimFree);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.REWARD_POPUP, RefreshOrder);
        EventDispatcher.Instance.AddEventListener(EventEnum.NOTICE_POPUP, RefreshOrder);
        EventDispatcher.Instance.AddEventListener(EventEnum.Daily_Pack_Time_REFRESH, OnTimeFinish);
        EventDispatcher.Instance.AddEventListener(EventEnum.NewDaily_Pack_Time_REFRESH, OnTimeFinishNewDailyPack);
        EventDispatcher.Instance.AddEventListener(EventEnum.NewDaily_Pack_REFRESH, OnTimeFinishNewDailyPack);
        EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_PURCHASE, OnBattlePassPurchase);
        EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, OnBattlePass2Purchase);

        AwakeVipStore();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Shop,AdLocalOperate.Open);
        
        if (objs != null && objs.Length > 0)
        {
            openSrc = (string) objs[0]; 
            firstArea = (ShowArea) objs[1];
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreOpen, openSrc,
                MainOrderManager.Instance.GetCurMaxTaskID().ToString());

            List<TableShop> lstCfgs = GlobalConfigManager.Instance.GetTableShop();
            StartCoroutine(ShowItems(lstCfgs,firstArea));
            Refresh();
            
            RefreshOrder(null);
        }
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }

        InitMainShopTab();
        InitVipTab();
        InitTeamShopTab();
        InvokeRepeating("UpdateTanBtnEnable",0,1);
    }
    public void UpdateTanBtnEnable()
    {
        TabBtnDic[TabState.MainStore].gameObject.SetActive(true);
        TabBtnDic[TabState.VipStore].gameObject.SetActive(VipStoreModel.Instance.IsOpenVipStore());
        TabBtnDic[TabState.TeamShop].gameObject.SetActive(TeamManager.Instance.HasOrder());
        var activeCount = 0;
        foreach (var pair in TabBtnDic)
        {
            if (pair.Value.gameObject.activeSelf)
                activeCount++;
        }
        transform.Find("Root/LabelGroup").gameObject.SetActive(activeCount > 0);
    }
    public void InitMainShopTab()
    {
        var vipStorePage = transform.Find("Root/LevelState2").gameObject.AddComponent<MainShopPage>();
        vipStorePage.Init();
        InitTab(TabState.MainStore,"LevelState2",vipStorePage);
    }

    public class MainShopPage : MonoBehaviour, ITabContent
    {
        public void Show()
        {
            Animator.PlayAnimation("Appear");
        }
        public void Hide()
        {
            Animator.PlayAnimation("Disappear");
        }
        private Transform Root;
        Animator Animator;
        public void Init()
        {
            Animator = transform.GetComponent<Animator>();
        }
    }
    

    private void RefreshOrder(BaseEvent e)
    {
        CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
        CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(false);
    }
     /// <summary>
    /// item初始化
    /// </summary>
    /// <param name="lstCfgs"></param>
    /// <returns></returns>
    private IEnumerator ShowItems(List<TableShop> lstCfgs,ShowArea firstArea=ShowArea.daily_reward)
    {
        _areaTitles.Clear();
        yield return new WaitForEndOfFrame();
        if (TaskAssistPackModel.Instance.GetTaskAssistPack().TaskAssistPacks.Count > 0 || GarageCleanupModel.Instance.CanShowPackInStore())
        {
            InitArea(lstCfgs, ShowArea.task_assist);
        }
        int level = ExperenceModel.Instance.GetLevel();
        Common cdata = AdConfigHandle.Instance.GetCommon();
        if (cdata.RvUnlock<=level)
            InitArea(lstCfgs, ShowArea.daily_reward);
        if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.FlashSale))
        {
            InitArea(lstCfgs, ShowArea.flash_sale);
        }
        InitBattlePassItem();
        InitPigItem();
        InitSealItem();
        InitDolpHinItem();
        InitEasterItem();
        InitArea(lstCfgs, ShowArea.gem_shop);
        
        if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.NewDailyPack) && NewDailyPackModel.Instance.IsOpen())
            InitArea(lstCfgs, ShowArea.NewDailyPack);
        if (UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyPack) && DailyPackModel.Instance.IsOpen())
             InitArea(lstCfgs, ShowArea.sale_of_day);
        
        InitArea(lstCfgs, ShowArea.daily_deals);
        if (firstArea != ShowArea.none)
        {
            var position = _content.localPosition;
            position.y = Math.Abs(GetArePos(firstArea).y) - 30;
            _content.localPosition = position;
        }
    }

     public void InitBattlePassItem()
     {
         if (Activity.BattlePass.BattlePassModel.Instance.IsOpened() &&! Activity.BattlePass.BattlePassModel.Instance.IsPurchase())
         {
             var sItem = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.ShopItemBattlePass).transform;
             var Item=Instantiate(sItem, _tsContentGroup);
             var itemScrips= Item.gameObject.AddComponent<UIStoreItemBattlePass>();
         }
         
         if (Activity.BattlePass_2.BattlePassModel.Instance.IsOpened() &&! Activity.BattlePass_2.BattlePassModel.Instance.IsPurchase())
         {
             var sItem = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.ShopItemBattlePass_2).transform;
             var Item=Instantiate(sItem, _tsContentGroup);
             var itemScrips= Item.gameObject.AddComponent<UIStoreItemBattlePass2>();
         }
     }

     public void InitPigItem()
     {
         if (PigBankModel.Instance.IsOpened() && PigBankModel.Instance.IsCanBuy())
         {
             var _curPigBankTable = PigBankModel.Instance.GetAdPigBankTable();
             TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(_curPigBankTable.ShopId);
             var pigItem = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.ShopItemPig).transform;
             var Item=Instantiate(pigItem, _tsContentGroup);
             var itemScrips= Item.gameObject.AddComponent<UIStorePigItem>();
             itemScrips.Init(tableShop,ShowArea.none,"shop");
         }
     }
     public void InitSealItem()
     {
         if (SealPackModel.Instance.CanShowInStore() )
         {
             var data = SealPackModel.Instance.BundleData;
             TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(data.shopItemId);
             var sItem = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.ShopItemSeal).transform;
             var Item=Instantiate(sItem, _tsContentGroup);
             var itemScrips= Item.gameObject.AddComponent<UIStoreSealItem>();
             itemScrips.Init(tableShop,ShowArea.none);
         }
     }    
     public void InitDolpHinItem()
     {
         if (DolphinPackModel.Instance.CanShowInStore() )
         {
             var data = DolphinPackModel.Instance.BundleData;
             TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(data.shopItemId);
             var sItem = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.ShopItemDolphin).transform;
             var Item=Instantiate(sItem, _tsContentGroup);
             var itemScrips= Item.gameObject.AddComponent<UIStoreDolphinItem>();
             itemScrips.Init(tableShop,ShowArea.none);
         }
     }
     
     public void InitEasterItem()
     {
         if (EasterPackModel.Instance.CanShowInStore() )
         {
             var data = EasterPackModel.Instance.BundleData;
             TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(data.shopItemId);
             var sItem = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.ShopItemEaster).transform;
             var Item=Instantiate(sItem, _tsContentGroup);
             var itemScrips= Item.gameObject.AddComponent<UIStoreEasterItem>();
             itemScrips.Init(tableShop,ShowArea.none);
         }
     }
     public Vector3 GetArePos(ShowArea firstArea)
     {
         if (_areaTitles.ContainsKey(firstArea))
             return _areaTitles[firstArea].transform.localPosition;

         return Vector3.zero;
     }
    public void InitArea(List<TableShop> lstCfgs, ShowArea area)
    {
        var titleObj = Instantiate(_giftTitle, _tsContentGroup);
        titleItem = titleObj.gameObject.AddComponent<GameGiftTitleItem>();
        titleItem.Init(area);
        _areaTitles.Add(area, titleItem);
        List<TableShop> list = null;
        switch (area)
        {
            case ShowArea.task_assist:
                var packs = TaskAssistPackModel.Instance.GetTaskAssistPack();
                int count = packs.TaskAssistPacks.Count;
                if (GarageCleanupModel.Instance.CanShowPackInStore())
                {
                    count++;
                }
                titleItem.InitPageScrollView(count);
                if (GarageCleanupModel.Instance.CanShowPackInStore())
                {
                    InitGarageCleanBundle(titleItem, titleItem.SpecialContentTransform);
                }
                InitAllTaskAssistBundle(titleItem, titleItem.SpecialContentTransform, ShowArea.task_assist);
                break;
            case ShowArea.flash_sale:
                InitAllFlashShopItem(titleItem.NormalContentTransform, ShowArea.flash_sale);
                break;
            case ShowArea.daily_reward:
                InitAllPigShopItem(titleItem.NormalContentTransform, ShowArea.daily_reward);
                break;
            case ShowArea.gem_shop:
                list = lstCfgs.FindAll(x => x.productType == (int)StoreModel.eProductType.Diamond);
                for (int i = 0; i < list.Count; i++)
                {
                    InitShopItem<UIStoreNormalItem>(ObjectPoolName.ShopItemNomalPath, titleItem.DiamondContentTransform,
                        list[i]);
                }

                break;
            case ShowArea.coin_shop:
                var list1 = lstCfgs.FindAll(x => x.productType == (int)StoreModel.eProductType.Gold);
                for (int i = 0; i < list1.Count; i++)
                {
                    InitShopItem<UIStoreExchangeItem>(ObjectPoolName.ShopItemExchangePath,
                        titleItem.NormalContentTransform, list1[i]);
                }

                break;
            case ShowArea.energy:
                list = lstCfgs.FindAll(x => x.productType == 22);
                for (int i = 0; i < list.Count; i++)
                {
                    InitShopItem<UIStoreExchangeItem>(ObjectPoolName.ShopItemExchangePath,
                        titleItem.NormalContentTransform, list[i]);
                }

                break;
            case ShowArea.daily_deals:

                var list2 = lstCfgs.FindAll(x => x.productType == 100 && x.id!=100005);
                for (int i = 0; i < list2.Count; i++)
                {
                    InitShopItem<UIStoreDailyItem>(ObjectPoolName.ShopItemDailyPath, titleItem.NormalContentTransform,
                        list2[i]);
                }

                break;
            case ShowArea.sale_of_day:
            {
                var packInfo = DailyPackModel.Instance.packData.PackInfo;
                String source = "shop";
                for (int i = 0; i < packInfo.Count; i++)
                {
                    var packItem = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ShopItemBundleNomalPath)
                        .AddComponent<UIDailyBundleItem>();
                    packItem.transform.SetParent(titleItem.BundleContentTransform);
                    packItem.transform.localScale = Vector3.one;

                    var pi = AdConfigHandle.Instance.GetDailyPackInfoById(packInfo[i].PackInfoId);
                    packItem.Init(packInfo[i], pi, source);
                    _bundleItems.Add(packItem);
                }

                break;
            }
            case ShowArea.NewDailyPack:
            {
                String source = "shop";
                var packInfoList = NewDailyPackModel.Instance.packData.PackInfo;
                for (var i = 0; i < packInfoList.Count; i++)
                {
                    var packInfo = packInfoList[i];//NewDailyPackModel.Instance.GetCurrentPackage();
                    if (packInfo != null)
                    {
                        var packItem = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ShopItemBundleNormal1Path).AddComponent<UINewDailyBundleItem>();
                        packItem.transform.SetParent(titleItem.BundleContentTransform);
                        packItem.transform.localScale=Vector3.one;
                        var pi = GlobalConfigManager.Instance.GetNewDailyPackPackageConfig(packInfo.PackInfoId);
                        packItem.Init(packInfo, pi, source);
                        _bundleItemsNew.Add(packItem);
                    }   
                }
                break;   
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
    }
    private void InitAllFlashShopItem(Transform parent, ShowArea area) //  初始化每日商品
    {
        flashShopItems.Clear();
        int index = 0;
        var storeageItems = StoreModel.Instance.GetFlashSaleItems();
        for (int i = 0; i < storeageItems.Items.Count; i++)
        {
            index = i;
            InitFlashSaleShopItem<UIStoreFlashItem>(ObjectPoolName.ShopItemFlashPath, parent,
                storeageItems.Items[i], index, area);
        }
    }

    private void InitAllPigShopItem(Transform parent, ShowArea area)
    {
        ClimbTowerModel.Instance.InitShopEntry(parent);
        
        pigShopItems.Clear();
        int index = 0;
        var storeageItems = StoreModel.Instance.GetPigSaleItems();
        for (int i = 0; i < storeageItems.Items.Count; i++)
        {
            index = i;
            InitPigSaleShopItem<UIStorePigSaleItem>(ObjectPoolName.ShopItemFlashPath, parent,
                storeageItems.Items[i], index, area);
        }
    }

    
    private void InitAllTaskAssistBundle(GameGiftTitleItem gameGiftTitleItem, Transform parent, ShowArea area)
    {
        var packs = TaskAssistPackModel.Instance.GetTaskAssistPack();
        _dicTaskAssistItems.Clear();
        foreach (var packData in packs.TaskAssistPacks)
        {
            var packConfig = AdConfigHandle.Instance.GetTaskAssistPackById(packData.PackId);
            var cfg = GlobalConfigManager.Instance.GetTableShopByID(packConfig.ShopItem);
            Transform item = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TaskAssistBundlePath).transform;
            item.SetParent(parent);
            item.localScale = Vector3.one;
            item.localPosition = Vector3.zero;
            UIStoreTaskAssistBundleItem si = item.gameObject.GetComponent<UIStoreTaskAssistBundleItem>();
            if (null == si)
                si = item.gameObject.AddComponent<UIStoreTaskAssistBundleItem>();
            if(cfg!=null)
                si.Init(cfg, packConfig, gameGiftTitleItem, (ShowArea) cfg.area, openSrc);
            _dicTaskAssistItems.Add(packConfig.Id, si);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
        }

        gameGiftTitleItem.ResetPageScrollView();
    }

    
    private void InitGarageCleanBundle(GameGiftTitleItem gameGiftTitleItem, Transform parent)
    {
        var cfg = GlobalConfigManager.Instance.GetTableShopByType((int)StoreModel.eProductType.GarageCleanBundle);
        for (int i = 0; i < cfg.Count; i++)
        {
            var tableBundle = GlobalConfigManager.Instance.GetTableBundleByShopID(cfg[i].id);
            Transform item = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.FishBundlePath).transform;
            item.SetParent(parent);
            item.localScale = Vector3.one;
            item.localPosition = Vector3.zero;
            UIStoreGarageCleanBundleItem si = item.gameObject.GetComponent<UIStoreGarageCleanBundleItem>();
            if (null == si)
                si = item.gameObject.AddComponent<UIStoreGarageCleanBundleItem>();
            if(cfg!=null)
                si.Init(cfg[i], tableBundle, gameGiftTitleItem, (ShowArea) cfg[i].area, openSrc);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
        }
    }

    
    /// <summary>
    /// 初始化商店UIItem
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="parent">item父节点</param>
    /// <param name="cfg">商店配置</param>
    /// <typeparam name="T">控制脚本</typeparam>
    private void InitShopItem<T>(string path, Transform parent, TableShop cfg) where T : UIStoreBaseItem
    {

        Transform item = GamePool.ObjectPoolManager.Instance.Spawn(path).transform;
        item.SetParent(parent);
        item.localScale = Vector3.one;
        item.localPosition = Vector3.zero;
        T si = item.gameObject.GetComponent<T>();
        if (null == si)
            si = item.gameObject.AddComponent<T>();
        si.Init(cfg, (ShowArea) cfg.area, openSrc);
        // item.gameObject.SetActive(false);
        _dicItems.Add(cfg.id, si);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
    }
    private void InitFlashSaleShopItem<T>(string path, Transform parent, StorageStoreItem storeItem, int index,
        ShowArea area) where T : UIStoreBaseItem
    {
        Transform item = GamePool.ObjectPoolManager.Instance.Spawn(path).transform;
        item.SetParent(parent);
        item.localScale = Vector3.one;
        item.localPosition = Vector3.zero;
        T si = item.gameObject.GetComponent<T>();
        if (null == si)
            si = item.gameObject.AddComponent<T>();
        si.Init(null, ShowArea.flash_sale, openSrc);
        (si as UIStoreFlashItem).SetItemId(storeItem, index, true);
        si.Refresh();
        flashShopItems.Add((si as UIStoreFlashItem));
    }

    private void InitPigSaleShopItem<T>(string path, Transform parent, StorageStoreItem storeItem, int index,
        ShowArea area) where T : UIStoreBaseItem
    {
        Transform item = GamePool.ObjectPoolManager.Instance.Spawn(path).transform;
        item.SetParent(parent);
        item.localScale = Vector3.one;
        item.localPosition = Vector3.zero;
        T si = item.gameObject.GetComponent<T>();
        if (null == si)
            si = item.gameObject.AddComponent<T>();
        si.Init(null, ShowArea.flash_sale, openSrc);
        (si as UIStorePigSaleItem).SetItemId(storeItem, index, true,firstArea);
        si.Refresh();
        // item.gameObject.SetActive(false);
        pigShopItems.Add((si as UIStorePigSaleItem));
    }

    private void OnPurchase(BaseEvent evt)
    {
        int data = (int) evt.datas[0];
        if (_dicItems.ContainsKey(data))
        {
            _dicItems[data].Refresh();
        }
    }

    private void OnClaimFree(BaseEvent evt)
    {
        int data = (int) evt.datas[0];
        if (_dicItems.ContainsKey(data))
        {
            _dicItems[data].Refresh();
        }
    }
    
    private void Refresh()
    {
        _csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
    }

    private void OnClickClose()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreClose, openSrc,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString());

        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Shop,AdLocalOperate.Close);
        EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate);
    }

    public static void OpenUI(string src = "",ShowArea area=ShowArea.none)
    {
        UIWindow uiView = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIStore);
        if (uiView == null)
            UIManager.Instance.OpenUI(UINameConst.UIStore, src,area);
    }
    public void ReBuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
    }
    public void RefreshFlashShop()
    {
        var items = StoreModel.Instance.GetFlashSaleItems();
        int index = 0;
        for (int i = 0; i < items.Items.Count; i++)
        {
            index = i;
            if (i >= flashShopItems.Count)
            {
            }
            else
            {
                flashShopItems[i].gameObject.SetActive(true);
                flashShopItems[i].SetItemId(items.Items[i], index);
                flashShopItems[i].Refresh();
            }
        }
    }
    
    public void RefreshDailyShop()
    {
        foreach (var kv in _dicItems)
        {
            var item = StoreModel.Instance.GetStorageItem(kv.Value.ID);
            if (item != null)
            {
                item.BuyCount = 0;
                item.PriceAdd = 0;
                item.RvWatched = 0;
            }

            kv.Value.Refresh();
        }
    }
    private void OnTimeFinish(BaseEvent obj)
    {
        var packInfo = DailyPackModel.Instance.packData.PackInfo;
        for (int i = 0; i < _bundleItems.Count; i++)
        {
            var pi = AdConfigHandle.Instance.GetDailyPackInfoById(packInfo[i].PackInfoId);
            _bundleItems[i].Init(packInfo[i],pi,"shop");
        }
     
    }
    private void OnTimeFinishNewDailyPack(BaseEvent obj)
    {
        var packInfo = NewDailyPackModel.Instance.packData.PackInfo;
        for (int i = 0; i < _bundleItemsNew.Count; i++)
        {
            var pi = GlobalConfigManager.Instance.GetNewDailyPackPackageConfig(packInfo[i].PackInfoId);
            _bundleItemsNew[i].Init(packInfo[i],pi,"shop");
        }
        
        // var packInfo = NewDailyPackModel.Instance.GetCurrentPackage();
        // String source = "shop";
        // var packItem = _bundleItemsNew.Count > 0 ? _bundleItemsNew[0] : null;
        // if (packInfo != null && packItem != null)
        // {
        //     var pi = GlobalConfigManager.Instance.GetNewDailyPackPackageConfig(packInfo.PackInfoId);
        //     packItem.Init(packInfo, pi, source);
        // }
        // if (packInfo == null && packItem != null)
        // {
        //     GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemBundleNormal1Path,packItem.gameObject);
        //     Destroy(packItem);
        //     if (_areaTitles.TryGetValue(ShowArea.NewDailyPack, out var title))
        //     {
        //         Destroy(title.gameObject);
        //     }
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
        // }
    }

    private void OnBattlePassPurchase(BaseEvent obj)
    {
        AnimCloseWindow(() =>
        {
            if (Activity.BattlePass.BattlePassModel.Instance.IsOpened())
                UIManager.Instance.OpenUI(UINameConst.UIBattlePassMain);
        });
    }
    
    private void OnBattlePass2Purchase(BaseEvent obj)
    {
        AnimCloseWindow(() =>
        {
            if (Activity.BattlePass_2.BattlePassModel.Instance.IsOpened())
                UIManager.Instance.OpenUI(UINameConst.UIBattlePass2Main);
        });
    }

    void OnDestroy()
    {
        DestroyVipStore();
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HOTEL_PURCHASE_SUCCESS, OnPurchase);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HOTEL_CLAIM_FREE_SUCCESS, OnClaimFree);
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.REWARD_POPUP, RefreshOrder);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NOTICE_POPUP, RefreshOrder);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.Daily_Pack_Time_REFRESH, OnTimeFinish);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NewDaily_Pack_Time_REFRESH, OnTimeFinishNewDailyPack);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NewDaily_Pack_REFRESH, OnTimeFinishNewDailyPack);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_PURCHASE, OnBattlePassPurchase);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, OnBattlePass2Purchase);

        CurrencyGroupManager.Instance?.currencyController?.RecoverCanvasSortOrder();
        CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(true);
        foreach (var kv in _dicItems)
        {
            switch (kv.Value.ItemType)
            {
                case UIStoreItemType.Normal:
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemNomalPath, kv.Value.gameObject);
                    break;
                case UIStoreItemType.Bundle:
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemBundleNomalPath,
                        kv.Value.gameObject);
                    break;
                case UIStoreItemType.Daily:
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemDailyPath, kv.Value.gameObject);
                    break;
                case UIStoreItemType.Flash:
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemFlashPath, kv.Value.gameObject);
                    break;
                case UIStoreItemType.Exchange:
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemExchangePath,
                        kv.Value.gameObject);
                    break;
            }
        }
        
        foreach (var kv in _dicTaskAssistItems)
        {
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TaskAssistBundlePath, kv.Value.gameObject);
        }
        
        foreach (var uiStoreFlashItem in flashShopItems)
        {
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemFlashPath, uiStoreFlashItem.gameObject);
            DestroyImmediate(uiStoreFlashItem);
        }
        
        foreach (var uiStoreFlashItem in pigShopItems)
        {
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemFlashPath, uiStoreFlashItem.gameObject);
            DestroyImmediate(uiStoreFlashItem);
        }
    }
}