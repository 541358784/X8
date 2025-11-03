
using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using RecoverCoin;
using UnityEngine;
using UnityEngine.UI;
public class UIRecoverCoinBuyController:UIWindowController
{
    private bool IsInBind => _storageWeek != null;
    private StorageRecoverCoinWeek _storageWeek;
    private Button _buttonPlay;
    private Button _buttonClose;
    private NormalRankNode _myRankNode;
    private LocalizeTextMeshProUGUI _priceText;
    private LocalizeTextMeshProUGUI _priceGreyText;
    private LocalizeTextMeshProUGUI _valueText;

    RecoverCoinExchangeStarConfig CurExchangeConfig
    {
        get
        {
            var cfgList = _storageWeek.ExchangeStarConfig();
            if (_storageWeek.BuyTimes >= cfgList.Count)
                return cfgList.Last();
            return cfgList[_storageWeek.BuyTimes];
        }
    }

    private int CurValue => CurExchangeConfig.Star;
    private int CurPrice => CurExchangeConfig.Coin;
    private long CurTime => (long) APIManager.Instance.GetServerTime();
    public override void PrivateAwake()
    {
        _buttonPlay = GetItem<Button>("Root/BuyGroup/Button");
        _buttonPlay.onClick.AddListener(OnPlayBtn);
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _myRankNode = new NormalRankNode(transform.Find("Root/Top"));
        _priceText = GetItem<LocalizeTextMeshProUGUI>("Root/BuyGroup/Button/Text");
        _priceGreyText = GetItem<LocalizeTextMeshProUGUI>("Root/BuyGroup/Button/GreyText");
        _valueText = GetItem<LocalizeTextMeshProUGUI>("Root/BuyGroup/Num");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CurrencyGroupManager.Instance?.currencyController?.SetCoinCanvasSortOrder(canvas.sortingOrder + 1);
        if (_buttonPlay.GetComponent<ShieldButtonOnClick>() != null)
        {
            _buttonPlay.GetComponent<ShieldButtonOnClick>().isUse = false;   
        }
    }

    void OnPlayBtn()
    {
        if (!IsInBind)
            return;
        if (CurTime > _storageWeek.EndTime)
            return;
        if (!UserData.Instance.CanAford(UserData.ResourceId.Coin, CurPrice))
        {
            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Coin, "RecoverCoinBuyStar",
                CurPrice.ToString(), "RecoverCoinBuyStar");
            return;
        }
        else
        {
            UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, CurPrice,
                new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CoinoverUse});
            var addStarCount = CurValue;
            RecoverCoinModel.FlyStar(CurValue, _buttonPlay.transform.position, _myRankNode._myNode._starIcon, 0.8f, true,
                action: () =>
                {
                    RecoverCoinModel.Instance.AddStar(addStarCount);
                    // UserData.Instance.AddRes((int) UserData.ResourceId.RecoverCoinStar, addStarCount, new GameBIManager.ItemChangeReasonArgs
                    // {
                    //     reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CoinoverGet
                    // }, false);
                });
            DragonPlus.GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinoverStartclick,
                data1: _storageWeek.BuyTimes.ToString(),
                data2: CurValue.ToString(),
                data3: CurPrice.ToString());
            _storageWeek.BuyTimes++;
            RefreshUI();
        }
    }

    void OnCloseBtn()
    {
        CloseWindowWithinUIMgr();
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        CurrencyGroupManager.Instance?.currencyController?.RecoverCoinCanvasSortOrder();
    }

    public void BindStorageWeek(StorageRecoverCoinWeek storageWeek)
    {
        if (_storageWeek == storageWeek)
        {
            RefreshUI();
            return;
        }
        if (_storageWeek != null)
            _storageWeek.SortController().UnBindRankChangeAction(OnMyRankChange);
        _storageWeek = storageWeek;
        _myRankNode.BindPlayer(_storageWeek.SortController().Me);
        _storageWeek.SortController().BindRankChangeAction(OnMyRankChange);
        RefreshUI();
    }

    public void OnMyRankChange(RecoverCoinPlayer player)
    {
        if (player.IsMe)
        {
            _myRankNode.BindPlayer(player);   
        }
    }
    public void RefreshUI()
    {
        if (!IsInBind)
            return;
        _priceText.SetText(CurPrice.ToString());
        _priceGreyText.SetText(CurPrice.ToString());
        _valueText.SetText(CurValue.ToString());
        _buttonPlay.interactable = UserData.Instance.CanAford(UserData.ResourceId.Coin, CurPrice);
    }

    private void OnDestroy()
    {
        if (_storageWeek != null)
            _storageWeek.SortController().UnBindRankChangeAction(OnMyRankChange);
    }
}