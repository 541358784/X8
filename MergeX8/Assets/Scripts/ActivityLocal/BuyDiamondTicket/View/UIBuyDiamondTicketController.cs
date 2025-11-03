using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIBuyDiamondTicketController:UIWindowController
{
    public static UIBuyDiamondTicketController Open(StorageBuyDiamondTicket storage)
    {
        var view = (UIManager.Instance.GetOpenedUIByPath<UIBuyDiamondTicketController>(UINameConst.UIBuyDiamondTicket)) ;
        if (view)
            return view;
        view = UIManager.Instance.OpenUI(UINameConst.UIBuyDiamondTicket,storage) as UIBuyDiamondTicketController;
        return view;
    }

    private LocalizeTextMeshProUGUI TimeText;
    private Button BuyBtn;
    private Button CloseBtn;
    private StorageBuyDiamondTicket Storage;
    
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        BuyBtn = GetItem<Button>("Root/Button");
        BuyBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                UIStoreController.OpenUI("BuyDiamondTicket",ShowArea.gem_shop);
            });
        });
        CloseBtn = GetItem<Button>("Root/CloseButton");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        InvokeRepeating("UpdateTime",0,1);
    }

    public void UpdateTime()
    {
        TimeText.SetText(Storage.GetTicketLeftTimeText());
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageBuyDiamondTicket;
    }
}