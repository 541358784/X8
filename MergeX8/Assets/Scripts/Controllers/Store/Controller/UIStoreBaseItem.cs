using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public enum UIStoreItemType
{
    Normal,
    Bundle,
    Flash,
    Exchange,
    Daily,
}

public enum ShowArea
{
    none = -1,
    gem_shop = 0, //钻石商店
    sale_of_day = 1,
    flash_sale = 2,
    coin_shop = 3, //金币商店
    daily_deals = 4,
    power_up = 5, //加速道具
    energy = 6, //体力
    box = 7, //每日箱子
    daily_reward = 8,
    task_assist = 9,
    NewDailyPack = 38,//新每日礼包
}

public abstract class UIStoreBaseItem : MonoBehaviour
{
    public int ID
    {
        get { return _cfg.id; }
    }

    public abstract UIStoreItemType ItemType { get; }

    private TableShop _cfg;

    public TableShop Cfg
    {
        get { return _cfg; }
    }

    private ShowArea _showArea;

    public ShowArea showArea
    {
        get { return _showArea; }
    }

    private string _openSrc;

    public string OpenSrc
    {
        get { return openSrc; }
    }

    public string openSrc;
    private StoreSaleConfig _saleCfg;

    public StoreSaleConfig SaleCfg
    {
        get { return _saleCfg; }
    }

    private void Awake()
    {
        PrivateAwake();
    }

    private void Start()
    {
        PrivateStart();
        if (null != BuyButton)
        {
            BuyButton.onClick.AddListener(OnClickBuy);
        }
    }

    public void Init(TableShop cfg, ShowArea area = ShowArea.none, string src = "")
    {
        _showArea = area;
        openSrc = src;
        if (cfg == null)
            return;
        _cfg = cfg;
        Refresh();
    }

    protected abstract void PrivateAwake();

    protected abstract void PrivateStart();

    protected void OnClickBuy()
    {
        if (showArea == ShowArea.daily_deals || showArea == ShowArea.daily_deals || showArea == ShowArea.flash_sale ||
            showArea == ShowArea.energy || showArea == ShowArea.coin_shop || showArea == ShowArea.power_up ||
            showArea == ShowArea.box)
            return;
        var extras = new Dictionary<string, string>();
        extras.Clear();
        extras.Add("type", "iap");
        extras.Add("amount", Cfg.price.ToString());
        string type = Cfg.productType == 3 ? "bundle" : "diamond";
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuy, OpenSrc,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), type, extras);

        int buyID = ID;

        AppConst.tempButRemoveAdsType = "store";
        AppConst.tempBuyStarPackType = "store";
        AppConst.tempBuySpecialPackType = "store";
        AppConst.tempBuyWeeklyCardType = "store";
        StoreModel.Instance.nowItemPos = this.transform.position;
        if(_cfg.productType==9)
        {       
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventSealPackagePurchase, "shop");
        } 
        if(_cfg.productType==10)
        {       
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventEasterPackagePurchase, "shop");
        }
        StoreModel.Instance.Purchase(buyID, openSrc);
        // GameBIManager.Instance.SendOneTimeBI(BiEventHotelFrenzy.Types.GameEventType.GameEventFirstRecharge, ClientMgr.Instance.MaxLevel.ToString(), isAuto: false);
    }

    protected abstract Button BuyButton { get; }

    public abstract void Refresh();
}