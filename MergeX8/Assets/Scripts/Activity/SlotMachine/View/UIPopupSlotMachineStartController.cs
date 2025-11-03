using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIPopupSlotMachineStartController:UIWindowController
{
    // private LocalizeTextMeshProUGUI TimeText;
    private Button StartBtn;
    private Button CloseBtn;
    private StorageSlotMachine Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageSlotMachine;
    }

    public override void PrivateAwake()
    {
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(OnClickStartBtn);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        // TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        // InvokeRepeating("UpdateTime",0f,1f);
        EventDispatcher.Instance.AddEvent<EventSlotMachineEnd>(OnActivityEnd);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventSlotMachineEnd>(OnActivityEnd);
    }

    public void OnActivityEnd(EventSlotMachineEnd evt)
    {
        AnimCloseWindow();
    }
    public void OnClickStartBtn()
    {
        AnimCloseWindow(() =>
        {
            UIPopupSlotMachineMainController.Open(Storage); 
        });
    }
    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    // public void UpdateTime()
    // {
    //     TimeText.SetText(SlotMachineModel.Instance.GetActivityLeftTimeString());
    // }
    public static UIPopupSlotMachineStartController Open(StorageSlotMachine storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupSlotMachineStart,storage) as UIPopupSlotMachineStartController;
    }
}