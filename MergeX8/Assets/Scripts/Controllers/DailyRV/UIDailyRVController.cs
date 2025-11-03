using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Game;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

public class UIDailyRVController : UIWindowController
{
        private readonly List<UIDailyRVItemController> _items = new List<UIDailyRVItemController>();
    private Button _buttonClose;
    private ScrollView _loopScrollView;
    private RectTransform _shopListTrans;
    private RectTransform _viewPortTrans;
    private UIDailyRVItemController _currentItem;
    private LocalizeTextMeshProUGUI _resetTimeText;
    private UIDailyRVItemController _rvItemTemplate;
    private Animator _animator;
    private Tween _tween;
    public static UIDailyRVController Instance;

    private bool isRefreshUI = false;
    private bool isGetReward = false;

    private List<RVshopResource> _rvShopResources;
    public override void PrivateAwake()
    {
        Instance = this;
        //CommonUtils.NotchAdapte(transform.Find("Root"));

        _buttonClose = GetItem<Button>("Root/TopGroup/CloseSelectButton");
        _buttonClose?.onClick.AddListener(OnCloseClicked);

        _resetTimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/TimeGroup/TextTime");
        _shopListTrans = GetItem<RectTransform>("Root/MiddleGroup/Scroll View/Viewport/Content");

        _loopScrollView = GetItem<ScrollView>("Root/MiddleGroup/Scroll View");
        _viewPortTrans = GetItem<RectTransform>("Root/MiddleGroup/Scroll View/Viewport");

        _animator = gameObject.GetComponent<Animator>();
        EventDispatcher.Instance.AddEventListener(EventEnum.M3_RV_SHOP_REFRESH, RefreshItems);
        EventDispatcher.Instance.AddEventListener(EventEnum.RV_SHOP_PURCHASE, PruchaseRefesh);
        EventDispatcher.Instance.AddEventListener(EventEnum.REWARD_POPUP, RewardPopup);
        EventDispatcher.Instance.AddEventListener(EventEnum.NOTICE_POPUP, RewardPopup);

        _rvShopResources = DailyRVModel.Instance.GetCurRVShopList();
        _loopScrollView.onItemRender.AddListener(OnItemRender);
        _loopScrollView.numItems = (uint)_rvShopResources.Count;
        
        UpdateScrollView(_rvShopResources, _items);
    }

    private void OnDestroy()
    {
        _tween?.Kill();
        _tween = null;
        EventDispatcher.Instance.RemoveEventListener(EventEnum.RV_SHOP_PURCHASE, PruchaseRefesh);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.M3_RV_SHOP_REFRESH, RefreshItems);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.REWARD_POPUP, RewardPopup);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NOTICE_POPUP, RewardPopup);
        CurrencyGroupManager.Instance?.currencyController?.RecoverCanvasSortOrder();
        CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(true);
    }

    private string source;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        source = "";//objs[0] as string
        InvokeRepeating("UpdateResetTime", 0, 1);

        RefreshItems(null);

        isRefreshUI = false;
        isGetReward = false;
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip,AdLocalOperate.Open);

        CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
        CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(false);
    }
    
    protected override void OnCloseWindow(bool destroy = false)
    {
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip,AdLocalOperate.Close,AdLocalSkipScene.RvShop);
        base.OnCloseWindow(destroy);
    }

    private void PruchaseRefesh(BaseEvent baseEvent)
    {
        if (baseEvent == null || baseEvent.datas == null || baseEvent.datas.Length == 0)
            return;

        TableShop tableShop = (TableShop) baseEvent.datas[0];
        if (tableShop == null)
            return;

        if (_currentItem == null)
            return;

        RVshopResource rvResData = _currentItem.Data;
        int elementIdx = _currentItem.Index;

        if (rvResData.ConsumeType != 4 || tableShop.id != rvResData.ConsumeAmount)
            return;

        OnRVShopItemSuc(rvResData, elementIdx, true);
    }

    private void RewardPopup(BaseEvent baseEvent)
    {
        CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
    }

    private void RefreshItems(BaseEvent baseEvent)
    {
        if (baseEvent != null)
        {
            isRefreshUI = true;
            if (!isGetReward)
            {
                UIManager.Instance.CloseUI(UINameConst.UIDailyRV);
                return;
            }
        }

        _rvShopResources = DailyRVModel.Instance.GetCurRVShopList();
        if (_rvShopResources == null || _rvShopResources.Count <= 0)
        {
            CloseWindowWithinUIMgr(true);
            return;
        }

        UpdateScrollView(_rvShopResources, _items);
    }

    private void UpdateCurrentItem(UIDailyRVItemController item)
    {
        if (this == null || gameObject == null || item == null || item.gameObject == null)
            return;

        _currentItem = item;
    }

    private void UpdateResetTime()
    {
        var t = DailyRVModel.Instance.GetCurRVShopListRestTimeString();
        _resetTimeText.SetText(t);
    }

    private void OnCloseClicked()
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    public bool OnRVAdClicked(RVshopResource data)
    {
        isGetReward = true;
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip,AdLocalOperate.Operate);


        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        if (_currentItem == null)
            return false;

        if (data != _currentItem.Data)
            return false;

        RVshopResource rvResData = _currentItem.Data;
        int elementIdx = _currentItem.Index;

        switch (rvResData.ConsumeType)
        {
            case 1:
            {
                AdSubSystem.Instance.PlayRV(ADConstDefine.RV_TV_REWARD, suc =>
                {
                    if (suc)
                    {
                        OnRVShopItemSuc(rvResData, elementIdx);
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRewardVideoDisplaySuccessDetails,
                            data1:ADConstDefine.RV_TV_REWARD,data2:"DailyRV Source="+source);
                    }
                }, true);
                break;
            }
            case 2:
            {
                if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, data.ConsumeAmount))
                {
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "DailyRvShop",
                        rvResData.Id.ToString(), "DailyRvShop",true,data.ConsumeAmount);

                    CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
                    return false;
                }
                else
                {
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, data.ConsumeAmount,
                        new GameBIManager.ItemChangeReasonArgs
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.RvshopGems
                        });
                    OnRVShopItemSuc(rvResData, elementIdx);
                }

                break;
            }
            case 3:
            {
                OnRVShopItemSuc(rvResData, elementIdx);
                break;
            }
            case 4:
            {
                StoreModel.Instance.Purchase(data.ConsumeAmount);
                break;
            }
            case 5:
            {
                if (!UserData.Instance.CanAford(UserData.ResourceId.Coin, data.ConsumeAmount))
                {
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Coin, "DailyRvShop",
                        rvResData.Id.ToString(), "DailyRvShop");

                    CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
                    return false;
                }
                else
                {
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, data.ConsumeAmount,
                        new GameBIManager.ItemChangeReasonArgs
                        {
                            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.RvshopCoins
                        });
                    OnRVShopItemSuc(rvResData, elementIdx);
                }

                break;
            }
        }

        return true;
    }

    private void UnLockNext(int index)
    {
        if (this == null || gameObject == null)
            return;

        StartCoroutine(Cor_UnLockNext(index));
    }

    private IEnumerator Cor_UnLockNext(int index)
    {
        int rvShopCount = _rvShopResources.Count;

        var current = FindItem(index);
        current?.ToDone();
        yield return new WaitForSeconds(0.5f);

        var next = FindItem(index+1);
        if (next != null)
        {
            next.ToUnLock(1);
            UpdateCurrentItem(next);
        }

        next = FindItem(index+2);
        if (next != null)
        {
            next.ToUnLock(2);
        }

        if(DailyRVModel.Instance.IsRVShopOpen())
            _loopScrollView.ScrollTo(index+1, 1f);
        
        yield return new WaitForSeconds(0.3f);
        next = FindItem(index+3);
        if (next != null)
        {
            next.ToUnLock(3);
        }

        yield return new WaitForSeconds(0.25f);

        if (DailyRVModel.Instance.IsRVShopOpen())
        {
            foreach (var kv in _items)
            {
                kv.RefreshState();
            }
        }
        
        if (!DailyRVModel.Instance.IsRVShopOpen())
        {
            UIManager.Instance.CloseUI(UINameConst.UIDailyRV, true);
            UIHomeMainController.UpdateAuixControllers();
            yield break;
        }
    }

    private UIDailyRVItemController FindItem(int index)
    {
        return _items.Find(a => a.Index == index);
    }
    private void OnRVShopItemSuc(RVshopResource data, int elementIdx, bool isPruchase = false)
    {
        isGetReward = false;

        var ret = new List<ResData>();
        for (int i = 0; i < data.RewardID.Count; i++)
            ret.Add(new ResData(data.RewardID[i], data.Amount[i]));
        
        if (!isRefreshUI)
            DailyRVModel.Instance.RVShopFinishItem(data, elementIdx);

        EventDispatcher.Instance.DispatchEvent(EventEnum.RV_SHOP_DATA_CHANGE);
        if (!isPruchase)
        {
            for (int i = 0; i < data.RewardID.Count; i++)
            {
                if (!UserData.Instance.IsResource(data.RewardID[i]))
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonTvReward,
                        itemAId = data.RewardID[i],
                        isChange = true,
                    });
                }
            }
            var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Adv);
            reasonArgs.data2 = ADConstDefine.RV_TV_REWARD;
            reasonArgs.data3 = data.Id.ToString();
            CommonRewardManager.Instance.PopCommonReward(ret, CurrencyGroupManager.Instance.currencyController, true,
                reasonArgs, () =>
                {
                    if (!isRefreshUI)
                    {
                        UnLockNext(elementIdx);
                    }
                    else
                    {
                        UIManager.Instance.CloseUI(UINameConst.UIDailyRV, true);
                    }
                });
        }
        else
        {
            if (!isRefreshUI)
            {
                UnLockNext(elementIdx);
            }
            else
            {
                UIManager.Instance.CloseUI(UINameConst.UIDailyRV, true);
            }
        }

        CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
    }

    public static bool CanShowUI()
    {
        if (DailyRVModel.Instance.Time2PopUpRVShop() == false)
            return false;
        UIDailyRVController.Open("Auto");
        return true;
    }

    public static UIDailyRVController Open(string source)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIDailyRV,source) as UIDailyRVController;
    }

    private void OnItemRender(int index, Transform child)
    {
        UIDailyRVItemController script = child.gameObject.GetComponent<UIDailyRVItemController>();
        if (script == null)
        {
            script = child.gameObject.AddComponent<UIDailyRVItemController>();
        }
        
        script.Init(_rvShopResources[index], index, canvas.sortingOrder);

        if(DailyRVModel.Instance.CurRvShopElementIdx > index)
            script.gameObject.SetActive(false);
        else
        {
            script.gameObject.SetActive(true);
        }
        if(index == DailyRVModel.Instance.CurRvShopElementIdx)
            UpdateCurrentItem(script);
        
        if(!_items.Contains(script))
            _items.Add(script);
    }

    private void UpdateScrollView(List<RVshopResource> resources, List<UIDailyRVItemController> listItems)
    {
        if(resources == null || resources.Count == 0)
            return;
        
        foreach (var kv in listItems)
        {
            kv.RefreshState();
        }
        
        _loopScrollView.ScrollTo(DailyRVModel.Instance.CurRvShopElementIdx, 0);
    }
}