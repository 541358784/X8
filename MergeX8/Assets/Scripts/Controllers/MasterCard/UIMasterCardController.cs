using System;
using System.Collections.Generic;
using System.Text;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIMasterCardController : UIWindowController
{
    private enum MasterCardType
    {
        NoAds = 1,
        PayDouble = 2,
        AddBag = 3,
        LessCD = 4,
        MaxEnergy = 5,
        AddProduct = 6,
    }

    private static string coolTimeKey = "masterCard";

    private Button _closeBtn;
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _timeText;
    private Dictionary<int, LocalizeTextMeshProUGUI> rewawrdTexList = new Dictionary<int, LocalizeTextMeshProUGUI>();

    private Button _buyButton1;
    private Text _buyButton1Tex;
    private Text _buyButton1ReduceTex;
    private LocalizeTextMeshProUGUI _buyButton1TagTex;
    private Transform _buyButton1ReduceGroup;

    private Button _buyButton2;
    private Text _buyButton2Tex;
    private Text _buyButton2ReduceTex;
    private LocalizeTextMeshProUGUI _buyButton2TagTex;
    private Transform _buyButton2ReduceGroup;

    private MasterCardList _masterCardList;
    private List<MasterCardResource> _masterCardDatas;

    // 1 去插屏
    // 2 充值钻石翻倍
    // 3 增加背包格数
    // 4 建筑cd减少
    // 5 体力上限增加

    public override void PrivateAwake()
    {
        MasterCardModel.Instance.InitMasterCard();

        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnClose);
        _timeGroup = transform.Find("Root/MiddleGroup/TimeGroup");
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/TimeGroup/TimeText");

        rewawrdTexList.Add((int) MasterCardType.LessCD,
            GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/RewardGroup/Item1/Text"));
        rewawrdTexList.Add((int) MasterCardType.AddBag,
            GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/RewardGroup/Item2/Text"));
        rewawrdTexList.Add((int) MasterCardType.AddProduct,
            GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/RewardGroup/Item3/Text"));
        rewawrdTexList.Add((int) MasterCardType.MaxEnergy,
            GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/RewardGroup/Item4/Text"));
        rewawrdTexList.Add((int) UserData.ResourceId.Diamond,
            GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/RewardGroup/Item5/Text"));
        rewawrdTexList.Add((int) MasterCardType.PayDouble,
            GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/RewardGroup/Item6/Text"));

        _buyButton1 = GetItem<Button>("Root/ButtonGroup/BuyButton1");
        _buyButton1.onClick.AddListener(() => { BuyMasterCard(0); });
        _buyButton1Tex = GetItem<Text>("Root/ButtonGroup/BuyButton1/Text");
        _buyButton1ReduceTex = GetItem<Text>("Root/ButtonGroup/BuyButton1/ReduceGroup/Text");
        _buyButton1TagTex = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonGroup/BuyButton1/TagGroup/Text");
        _buyButton1ReduceGroup = transform.Find("Root/ButtonGroup/BuyButton1/ReduceGroup");

        _buyButton2 = GetItem<Button>("Root/ButtonGroup/BuyButton2");
        _buyButton2.onClick.AddListener(() => { BuyMasterCard(1); });
        _buyButton2Tex = GetItem<Text>("Root/ButtonGroup/BuyButton2/Text");
        _buyButton2ReduceTex = GetItem<Text>("Root/ButtonGroup/BuyButton2/ReduceGroup/Text");
        _buyButton2TagTex = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonGroup/BuyButton2/TagGroup/Text");
        _buyButton2ReduceGroup = transform.Find("Root/ButtonGroup/BuyButton2/ReduceGroup");


        EventDispatcher.Instance.AddEventListener(EventEnum.MASTERCARD_PURCHASE, Mastercard_Purchase);
        EventDispatcher.Instance.AddEventListener(EventEnum.MASTERCARD_GETREWARD, Mastercard_GetReward);
        EventDispatcher.Instance.AddEventListener(EventEnum.REWARD_POPUP, RewardPopup);
        EventDispatcher.Instance.AddEventListener(EventEnum.NOTICE_POPUP, RewardPopup);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _masterCardList = MasterCardModel.Instance.GetMasterCardList();
        _masterCardDatas = MasterCardModel.Instance.GetMasterCardDatas();
        InitUI();
        UpdateUI();

        CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
        CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(false);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.MASTERCARD_PURCHASE, Mastercard_Purchase);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.MASTERCARD_GETREWARD, Mastercard_GetReward);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.REWARD_POPUP, RewardPopup);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NOTICE_POPUP, RewardPopup);
        CurrencyGroupManager.Instance?.currencyController?.RecoverCanvasSortOrder();
        CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(true);
    }

    private void InitUI()
    {
        if (_masterCardList == null)
            return;

        _buyButton1Tex.text = StoreModel.Instance.GetPrice(_masterCardList.Buy7_shopId);
        _buyButton1ReduceTex.text = GetLocalizedPriceString(_masterCardList.Org7_showPrice);
        _buyButton1TagTex.SetTerm("ui_master_card_7");

        _buyButton2Tex.text = StoreModel.Instance.GetPrice(_masterCardList.Buy30_shopId);
        _buyButton2ReduceTex.text = GetLocalizedPriceString(_masterCardList.Org30_showPrice);
        _buyButton2TagTex.SetTerm("ui_master_card_30");

        foreach (var kv in rewawrdTexList)
        {
            kv.Value.transform.parent.gameObject.SetActive(false);
        }

        if (_masterCardDatas == null)
            return;

        for (int i = 0; i < _masterCardDatas.Count; i++)
        {
            MasterCardResource data = _masterCardDatas[i];
            if (data == null)
                continue;

            if (!rewawrdTexList.ContainsKey(data.Type))
                continue;

            rewawrdTexList[data.Type].transform.parent.gameObject.SetActive(true);
            // if (!UserData.Instance.IsResource(data.Type))
            // {
            if (!data.Description.IsEmptyString())
                rewawrdTexList[data.Type]
                    .SetText(LocalizationManager.Instance.GetLocalizedStringWithFormats(data.Description,
                        data.RewardParam.ToString()));
            else
                rewawrdTexList[data.Type].SetText(data.RewardParam.ToString());
            // }
            // else
            // {
            //     rewawrdTexList[data.Type].SetText(data.RewardParam.ToString());
            // }
        }
    }

    private void UpdateUI()
    {
        bool isBuy = MasterCardModel.Instance.IsBuyMasterCard;
        _timeGroup.gameObject.SetActive(isBuy);

        if (isBuy)
        {
            CancelInvoke(((System.Action) UpdateResetTime).Method.Name);
            InvokeRepeating(((System.Action) UpdateResetTime).Method.Name, 0, 1);
        }
    }

    private void OnBtnClose()
    {
        if (MasterCardModel.Instance.IsCanPopupRenewal())
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                HasCancelButton = true,
                HasCloseButton = false,
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_master_card_end_desc"),
                OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_later"),
                CancelButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_yes"),
                OKCallback = () =>
                {
                    MasterCardModel.Instance.MasterCard.IsPopupRenewal = true;
                    CloseWindowWithinUIMgr(true);
                },
                CancelCallback = () => { }
            });
        }
        else
        {
            CloseWindowWithinUIMgr(true);
        }
    }

    private void UpdateResetTime()
    {
        var t = MasterCardModel.Instance.GetMasterCareLeftTime();
        _timeText.SetText(t);
    }

    private void Mastercard_Purchase(BaseEvent e)
    {
        UpdateUI();
    }

    private void Mastercard_GetReward(BaseEvent e)
    {
    }

    private void BuyMasterCard(int type)
    {
        if (_masterCardList == null)
            return;

        int shopId = -1;
        switch (type)
        {
            case 0:
            {
                shopId = _masterCardList.Buy7_shopId;
                break;
            }
            case 1:
            {
                shopId = _masterCardList.Buy30_shopId;
                break;
            }
        }

        if (shopId < 0)
            return;

        StoreModel.Instance.Purchase(shopId);
    }

    public static bool CanPopupReward()
    {
        return CanPopupReward(null);
    }

    public static bool CanPopupReward(Action endCall)
    {
        if (MasterCardModel.Instance.LeftRewardCount <= 0)
            return false;
        if (!MasterCardModel.Instance.IsOpen())
            return false;
        if (Utils.IsSameDay(MasterCardModel.Instance.MasterCard.GetRewardTime / 1000,
            (long) APIManager.Instance.GetServerTime() / 1000))
            return false;

        MasterCardModel.Instance.PopUpMasterCardReward(BiEventAdventureIslandMerge.Types.ItemChangeReason.Iap, "getreward", "",
            endCall);

        return true;
    }

    public static bool CanShowUI()
    {
        if (!MasterCardModel.Instance.IsOpen())
            return false;

        if (MasterCardModel.Instance.IsBuyMasterCard)
            return false;

        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
            return false;

        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,
            CommonUtils.GetTimeStamp());
        UIManager.Instance.OpenUI(UINameConst.UIMasterCard, "auto_main");
        return true;
    }

    public static bool CheckRenewal()
    {
        if (!MasterCardModel.Instance.IsOpen())
            return false;

        if (!MasterCardModel.Instance.IsCanPopupRenewal())
            return false;

        UIManager.Instance.OpenUI(UINameConst.UIMasterCard, "Renewal");
        return true;
    }

    private void RewardPopup(BaseEvent baseEvent)
    {
        CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
    }

    private string GetLocalizedPriceString(double priceInUSD)
    {
        double price = IAPUtils.GetLocalizedPrice(priceInUSD);

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0}{1}", IAPUtils.DeviceCurrencyCode, string.Format("{0:N2}", price));

        return sb.ToString();
    }
}