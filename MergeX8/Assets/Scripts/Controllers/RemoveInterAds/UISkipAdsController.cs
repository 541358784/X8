using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UISkipAdsController : UIWindowController
{
    private Animator _animator;
    private Button _buttonClose;
    private Button _buttonPurchase;
    private Button _buttonPurchased;
    private LocalizeTextMeshProUGUI _textPrice;

    private readonly int RemoveAdShopId = 99;


    public override void PrivateAwake()
    {
        _animator = GetComponent<Animator>();
        _buttonClose = GetItem<Button>("Root/UIBg/WindowsGroup/ButtonClose");
        _buttonPurchase = GetItem<Button>("Root/UIBg/WindowsGroup/ButtonBuy");
        _buttonPurchased = GetItem<Button>("Root/UIBg/WindowsGroup/ButtonObtained");
        _textPrice = GetItem<LocalizeTextMeshProUGUI>("Root/UIBg/WindowsGroup/ButtonBuy/TextPrice");

        _buttonClose?.onClick.AddListener(OnCloseClick);
        _buttonPurchase?.onClick.AddListener(OnPurchaseClick);

        EventDispatcher.Instance.AddEventListener(EventEnum.OnIAPItemPaid, OnIAPItemPaid);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OnIAPItemPaid, OnIAPItemPaid);
    }

    private void OnIAPItemPaid(BaseEvent obj)
    {
        RefreshPurchaseState();
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    private void RefreshPurchaseState()
    {
        _buttonPurchased?.gameObject.SetActive(StorageManager.Instance.GetStorage<StorageHome>().ShopData.GotNoAds);
        _buttonPurchase?.gameObject.SetActive(StorageManager.Instance.GetStorage<StorageHome>().ShopData.GotNoAds ==
                                              false);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _textPrice?.SetText(StoreModel.Instance.GetPrice(RemoveAdShopId));
        RefreshPurchaseState();
    }

    private void OnCloseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null,
            () => { CloseWindowWithinUIMgr(true); }));
    }

    private void OnPurchaseClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        StoreModel.Instance.Purchase(RemoveAdShopId);
        //StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        //{
        //    CloseWindowWithinUIMgr(true);
        //}));
    }
}