using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using TMatch;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreNormalItem : UIStoreBaseItem
{
    public enum StoreItemTagEnum
    {
        Normal = 0,
        BetterSale = 1,
        Hottest = 2,
        MostValuable = 3,
    }

    private Dictionary<StoreItemTagEnum, Transform> TagDic = new Dictionary<StoreItemTagEnum, Transform>();
    public override UIStoreItemType ItemType { get; } = UIStoreItemType.Normal;

    protected override Button BuyButton
    {
        get { return _btnBuy; }
    }

    private Button _btnBuy;

    private Image _icon;

    private Text _priceText;
    private Image _background;
    private LocalizeTextMeshProUGUI _contentText;
    private LocalizeTextMeshProUGUI _specialText;
    private LocalizeTextMeshProUGUI _specialContentText;
    private Transform _textGroup;
    private Transform ExtraReward;
    private LocalizeTextMeshProUGUI ExtraDiamondCountText;
    private Transform ExtraRewardTicket;
    private LocalizeTextMeshProUGUI ExtraTicketDiamondCountText;
    private LocalizeTextMeshProUGUI _vipText;

    protected override void PrivateAwake()
    {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _btnBuy = transform.Find("ButtonBuy").GetComponent<Button>();
        _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
        _contentText = transform.Find("Reward/ContentText").GetComponent<LocalizeTextMeshProUGUI>();
        _background = transform.Find("BG").GetComponent<Image>();
        _textGroup = transform.Find("TextGroup");
        _specialContentText = transform.Find("TextGroup/SpecialContentText").GetComponent<LocalizeTextMeshProUGUI>();
        _specialText = transform.Find("TextGroup/SpecialGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _vipText= transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _vipText.transform.parent.gameObject.SetActive(false);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OnIAPItemPaid, RefreshUI);
        EventDispatcher.Instance.AddEventListener(EventEnum.OnIAPItemPaid, RefreshUI);
        ExtraReward = transform.Find("ExtraReward");
        ExtraDiamondCountText = transform.Find("ExtraReward/ContentText").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.RemoveEvent<EventShopExtraRewardEnd>(OnShopExtraRewardEnd);
        EventDispatcher.Instance.AddEvent<EventShopExtraRewardEnd>(OnShopExtraRewardEnd);
        ExtraRewardTicket = transform.Find("ExtraRewardTicket");
        ExtraTicketDiamondCountText = transform.Find("ExtraRewardTicket/ContentText").GetComponent<LocalizeTextMeshProUGUI>();
        
        TagDic.Add(StoreItemTagEnum.Hottest, transform.Find("MostPopular"));
        TagDic.Add(StoreItemTagEnum.MostValuable, transform.Find("BestPrice"));
        var storeUI = UIStoreController.Instance;
        if (storeUI)
        {
            foreach (var pair in TagDic)
            {
                var sortingGroup = pair.Value.gameObject.AddComponent<Canvas>();
                sortingGroup.overrideSorting = true;
                sortingGroup.sortingLayerID = storeUI.canvas.sortingLayerID;
                sortingGroup.sortingOrder = storeUI.canvas.sortingOrder + 3;
            }
        }
    }

    private void RefreshUI(BaseEvent obj)
    {
        Refresh();
    }

    public void OnShopExtraRewardEnd(EventShopExtraRewardEnd evt)
    {
        Refresh();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OnIAPItemPaid, RefreshUI);
        EventDispatcher.Instance.RemoveEvent<EventShopExtraRewardEnd>(OnShopExtraRewardEnd);
    }

    protected override void PrivateStart()
    {
    }

    public override void Refresh()
    {
        var tag = (StoreItemTagEnum) Cfg.best_deal;
        foreach (var pair in TagDic)
        {
            pair.Value.gameObject.SetActive(pair.Key == tag);
        }

        _icon.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, Cfg.image);
        _priceText.text = (StoreModel.Instance.GetPrice(Cfg.id));
        
        _vipText.transform.parent.gameObject.SetActive(true);
        _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(Cfg.id));
        _contentText.SetText(Cfg.amount.ToString());
        // if (!string.IsNullOrEmpty(Cfg.background))
        //     _background.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, Cfg.background);
        if (MasterCardModel.Instance.IsBuyMasterCard && MasterCardModel.Instance.GetPayDouble() > 0)
        {
            _textGroup.gameObject.SetActive(true);
            _contentText.SetText((Cfg.amount * (100 + MasterCardModel.Instance.GetPayDoublePre()) / 100).ToString());
            _specialText.SetText(Cfg.amount.ToString());
        }
        else
        {
            _contentText.SetText(Cfg.amount.ToString());
            _textGroup.gameObject.SetActive(false);
        }
        
        var ticket = BuyDiamondTicketModel.Instance.GetActiveTicket();
        ExtraRewardTicket.gameObject.SetActive(ticket != null);
        if (ticket != null)
        {
            var config = ticket.GetTicketConfig();
            var diamondCount = config.GetExtraDiamondCount(Cfg.id);
            ExtraTicketDiamondCountText.SetText(diamondCount.ToString());   
        }
        
        var extraDiamondCount = GetExtraDiamond();
        ExtraReward.gameObject.SetActive(ticket == null && extraDiamondCount > 0);
        ExtraDiamondCountText.SetText(extraDiamondCount.ToString());
        
    }

    public int GetExtraDiamond()
    {
        var extraDiamond = 0;
        var extraRewards = ShopExtraRewardModel.Instance.GetExtraReward(Cfg.id);
        foreach (var resData in extraRewards)
        {
            if (resData.id == (int) UserData.ResourceId.Diamond)
            {
                extraDiamond += resData.count;
            }
        }

        return extraDiamond;
    }
}