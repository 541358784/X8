using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupThemeDecorationBuyPreEndController : UIWindowController
{

    public static UIPopupThemeDecorationBuyPreEndController Open(StorageThemeDecoration storageThemeDecoration)
    {
        return UIManager.Instance.OpenUI(storageThemeDecoration.GetAssetPathWithSkinName(UINameConst.UIPopupThemeDecorationBuyPreEnd), storageThemeDecoration) as
            UIPopupThemeDecorationBuyPreEndController;
    }

    public StorageThemeDecoration Storage;
    private LocalizeTextMeshProUGUI LeftTimeText;

    private Button _buyButton;
    private Text _buyText;
    private Button _noButton;
    private Button _buttonHelp;
    private Button _buttonClose;
    private Transform DefaultRewardItem;
    private List<CommonRewardItem> ItemList = new List<CommonRewardItem>();
    public override void PrivateAwake()
    {
        LeftTimeText = GetItem<LocalizeTextMeshProUGUI>("Root/Content/Text4");
        _buyButton = GetItem<Button>("Root/BuyButton");
        _buyText = GetItem<Text>("Root/BuyButton/Text1");
        _noButton = GetItem<Button>("Root/NoButton");
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonHelp = GetItem<Button>("Root/ButtonHelp");
        DefaultRewardItem = transform.Find("Root/Content/RewardGroup/Item");
        DefaultRewardItem.gameObject.SetActive(false);
        EventDispatcher.Instance.AddEvent<EventThemeDecorationBuySuccess>(PurchaseSuccess);
    }

    public int ShopId => ThemeDecorationModel.Instance.GlobalConfig.ExtendBuyShopId;

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageThemeDecoration;
        InvokeRepeating("UpdateTimeText",0,1);
        UpdateTimeText();
        _buyText.text = StoreModel.Instance.GetPrice(ShopId);
        _noButton.onClick.AddListener(OnBtnClose);
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonHelp.onClick.AddListener(OnBtnHelp);
        _buyButton.onClick.AddListener(OnBtnBuy);
        if (ThemeDecorationModel.Instance.IsInitFromServer())
        {
            var rewards = CommonUtils.FormatReward(ThemeDecorationModel.Instance.GlobalConfig.ExtendBuyRewardId,
                ThemeDecorationModel.Instance.GlobalConfig.ExtendBuyRewardNum);
            foreach (var reward in rewards)
            {
                var item = Instantiate(DefaultRewardItem.gameObject, DefaultRewardItem.parent).AddComponent<CommonRewardItem>();
                item.gameObject.SetActive(true);
                item.Init(reward);
                ItemList.Add(item);
            }
        }
    }

    private void PurchaseSuccess(EventThemeDecorationBuySuccess evt)
    {
        if (evt.Storage == Storage)
            AnimCloseWindow();
    }
    private void OnBtnBuy()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMermaidExtendExtendButton);

        StoreModel.Instance.Purchase(ShopId,"ThemeDecorationBuyMoreTime");
    }

    private void OnBtnHelp()
    {
        UIThemeDecorationHelpController.Open(Storage);
    }


    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void UpdateTimeText()
    {
        if (Storage.GetPreEndBuyLeftTime() < 0)
        {
            AnimCloseWindow();
        }
        else
        {
            LeftTimeText.SetTermFormats(Storage.GetPreEndBuyLeftTimeText());
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventThemeDecorationBuySuccess>(PurchaseSuccess);
    }
}