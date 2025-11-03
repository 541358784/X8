
using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;

public class UIPopupMermaidAddDayController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _text1;
    private LocalizeTextMeshProUGUI _text2;

    private Button _buyButton;
    private Text _buyText;
    private Button _noButton;
    private Button _buttonHelp;
    private Button _buttonClose;
    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/DayGroup/TimeGroup/TimeText");
        _text1 = GetItem<LocalizeTextMeshProUGUI>("Root/DayGroup/Text1");
        _text2 = GetItem<LocalizeTextMeshProUGUI>("Root/DayGroup/Text2");
        _buyButton = GetItem<Button>("Root/BuyButton");
        _buyText = GetItem<Text>("Root/BuyButton/Text1");
        _noButton = GetItem<Button>("Root/NoButton");
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonHelp = GetItem<Button>("Root/ButtonHelp");
        
        EventDispatcher.Instance.AddEventListener(EventEnum.MERMAID_PURCHASE_SUCCESS,PurchaseSuccess);
    }



    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        InvokeRepeating("UpdateTimeText",1,1);
        UpdateTimeText();
        _timeText.SetTerm("ui_event_mermaid_extend_time_desc2");

        var config = MermaidModel.Instance.MermaidConfig;
        _buyText.text = StoreModel.Instance.GetPrice(config.ExtendBuyShopId);
        _noButton.onClick.AddListener(OnBtnClose);
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonHelp.onClick.AddListener(OnBtnHelp);
        _buyButton.onClick.AddListener(OnBtnBuy);
    }

    private void PurchaseSuccess(BaseEvent obj)
    {
        AnimCloseWindow();
    }
    private void OnBtnBuy()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMermaidExtendExtendButton);

        StoreModel.Instance.Purchase(MermaidModel.Instance.MermaidConfig.ExtendBuyShopId);
    }

    private void OnBtnHelp()
    {
        UIManager.Instance.OpenUI(UINameConst.MermaidHelp);
    }


    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void UpdateTimeText()
    {
        if (MermaidModel.Instance.GetActivityExtendBuyWaitLeftTime() < 0)
        {
            AnimCloseWindow();
        }
        else
        {
            _text2.SetText(String.Format(LocalizationManager.Instance.GetLocalizedString("ui_event_mermaid_extend_time_desc4"),MermaidModel.Instance.GetActivityExtendBuyWaitLeftTimeString()));
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.MERMAID_PURCHASE_SUCCESS,PurchaseSuccess);
    }
}
