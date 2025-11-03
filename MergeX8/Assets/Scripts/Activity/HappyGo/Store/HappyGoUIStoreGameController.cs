using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using GamePool;
using UnityEngine;
using UnityEngine.UI;

public class HappyGoUIStoreGameController : UIWindow
{
    public static HappyGoUIStoreGameController Instance;
    private Button _btnClose;
    private Transform _tsContentGroup;
    private ScrollRect _scrollRect;

    private Dictionary<int, UIStoreBaseItem> _dicItems = new Dictionary<int, UIStoreBaseItem>();
    private List<HappyGoUIStoreFlashItem> flashShopItems = new List<HappyGoUIStoreFlashItem>();
    private ContentSizeFitter _csf;
    private SimpleGameObjectPool _pool;

    private Transform _giftTitle;
    private Transform _happyGoFlash;
    private Transform _bundle1;
    private Transform _bundle2;
    private Animator _animator;
    private string src = "";
    private GameGiftTitleItem titleItemDr;

    public override void PrivateAwake()
    {
        Instance = this;

        CommonUtils.NotchAdapte(transform.Find("Root") as RectTransform);
        _animator = gameObject.GetComponent<Animator>();
        _btnClose = transform.Find("Root/CloseButton").GetComponent<Button>();
        _tsContentGroup = transform.Find("Root/LevelState2/Viewport/Content");
        _scrollRect = transform.Find("Root/LevelState2").GetComponent<ScrollRect>();
        _csf = _tsContentGroup.GetComponent<ContentSizeFitter>();

        _giftTitle = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.HappyGoGiftTitlePath).transform;
        _happyGoFlash = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.HappyGoShopFlashPath).transform;
        _bundle1 = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.HappyGoShopBundlePath).transform;
        _bundle2 = ResourcesManager.Instance.LoadResource<GameObject>(ObjectPoolName.HappyGoShopBundlePath2).transform;
        _btnClose.onClick.AddListener(OnClickClose);

        List<TableShop> lstCfgs = GlobalConfigManager.Instance.GetTableShop();
        ShowItems(lstCfgs);
        EventDispatcher.Instance.AddEventListener(EventEnum.HOTEL_PURCHASE_SUCCESS, OnPurchase);

        EventDispatcher.Instance.AddEventListener(EventEnum.REWARD_POPUP, RefreshOrder);
        EventDispatcher.Instance.AddEventListener(EventEnum.NOTICE_POPUP, RefreshOrder);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        if (objs != null && objs.Length > 0)
            src = (string) objs[0];

        bool isResGet = false;
        if (objs != null && objs.Length >= 2)
            isResGet = (bool) objs[1];

        Refresh();

        if (isResGet)
        {
            _scrollRect.normalizedPosition = new Vector2(1, 0);
        }

        RefreshOrder(null);
    }

    /// <summary>
    /// item初始化
    /// </summary>
    /// <param name="lstCfgs"></param>
    /// <returns></returns>
    private void ShowItems(List<TableShop> lstCfgs)
    {
        HappyGoPackModel.Instance.GetHappyGoBundleItems();
        InitArea(lstCfgs,ShowArea.sale_of_day); 
        InitArea(lstCfgs, ShowArea.flash_sale);
    }

    private void RefreshOrder(BaseEvent e)
    {
        CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
        CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(false);
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate);
    }

    /// <summary>
    /// 初始化商店UIItem
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="parent">item父节点</param>
    /// <param name="cfg">商店配置</param>
    /// <typeparam name="T">控制脚本</typeparam>
    private void InitShopBundleItem( Transform parent, TableShop cfg, StorageHGBundleItem bundleItem,int type,
        HappyGoGameGiftTitleItem gameGiftTitleItem)
    {
        Transform item=null;
        if(type==1){
             item= Instantiate(_bundle1, parent);
        }
        else
        {
            item= Instantiate(_bundle2, parent);
        }
        item.SetParent(parent);
        item.localScale = Vector3.one;
        item.localPosition = Vector3.zero;
        UIHappyGoBundleItem si = item.gameObject.GetComponent<UIHappyGoBundleItem>();
        if (null == si)
            si = item.gameObject.AddComponent<UIHappyGoBundleItem>();
        si.Init(cfg, bundleItem,type, gameGiftTitleItem);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
    }

    private void InitFlashSaleShopItem<T>(Transform parent, StorageStoreItem storeItem, int index,
        HappyGoGameGiftTitleItem titleItem) where T : UIStoreBaseItem
    {
        Transform item = Instantiate(_happyGoFlash, parent);
        item.localScale = Vector3.one;
        item.localPosition = Vector3.zero;
        T si = item.gameObject.GetComponent<T>();
        if (null == si)
            si = item.gameObject.AddComponent<T>();
        si.Init(null, ShowArea.flash_sale, "");
        (si as HappyGoUIStoreFlashItem).SetItemId(storeItem, index, true,titleItem);
        si.Refresh();
        flashShopItems.Add((si as HappyGoUIStoreFlashItem));
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

    public void InitArea(List<TableShop> lstCfgs, ShowArea area)
    {
        if (area ==ShowArea.sale_of_day && HappyGoPackModel.Instance.GetCanBuyCount() <= 0)
            return;
        
        var titleObj = Instantiate(_giftTitle, _tsContentGroup);
        var titleItem = titleObj.gameObject.AddComponent<HappyGoGameGiftTitleItem>();
        titleItem.Init(area);
        switch (area)
        {
            case ShowArea.flash_sale:

                InitAllFlashShopItem(titleItem.NormalContentTransform, ShowArea.flash_sale,titleItem);
                break;
            case ShowArea.sale_of_day:
                var bundles = HappyGoPackModel.Instance.GetHappyGoBundleItems();
                foreach (var key in bundles.Keys)
                {
                    var shopCfg = GlobalConfigManager.Instance.GetTableShopByID(bundles[key].ShopId);
                    InitShopBundleItem( titleItem.BundleContentTransform, shopCfg,
                        bundles[key],key, titleItem);
                    titleItem.InitPageScrollView(bundles.Count);

                }
                break;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
    }

    private void InitAllFlashShopItem(Transform parent, ShowArea area,HappyGoGameGiftTitleItem titleItem) //  初始化每日商品
    {
        flashShopItems.Clear();
        int index = 0;
        var storeageItems = HappyGoStoreModel.Instance.GetFlashSaleItems();
        for (int i = 0; i < storeageItems.Items.Count; i++)
        {
            index = i;
            InitFlashSaleShopItem<HappyGoUIStoreFlashItem>(parent, storeageItems.Items[i], index, titleItem);
        }
    }

    private void Refresh()
    {
        _csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
    }

    public void ReBuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
    }

    private void OnClickClose()
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    public static void OpenUI(string src, bool isGetRes = false)
    {
        UIManager.Instance.OpenUI(UINameConst.HappyGoUIStoreGam, src, isGetRes);
    }

    void OnDestroy()
    {
        foreach (var kv in _dicItems)
        {
            switch (kv.Value.ItemType)
            {
                case UIStoreItemType.Bundle:
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.HappyGoShopBundlePath,
                        kv.Value.gameObject);
                    break;
                case UIStoreItemType.Flash:
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ShopItemFlashPath, kv.Value.gameObject);
                    break;
            }
        }

        CurrencyGroupManager.Instance?.currencyController?.RecoverCanvasSortOrder();
        CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(true);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.REWARD_POPUP, RefreshOrder);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NOTICE_POPUP, RefreshOrder);
    }

    public void RefreshFlashShop()
    {
        var items = HappyGoStoreModel.Instance.GetFlashSaleItems();
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
}