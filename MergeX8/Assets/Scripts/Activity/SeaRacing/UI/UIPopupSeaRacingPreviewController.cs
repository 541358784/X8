using System;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSeaRacingPreviewController:UIWindowController
{
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI TimeText;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime", 0f, 1.0f);
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public void UpdateTime()
    {
        if (Storage == null)
        {
            return;
        }

        if (Storage.GetPreheatLeftTime() <= 0)
        {
            TimeText.SetText(Storage.GetPreheatLeftTimeText());
            CancelInvoke("UpdateTime");
            AnimCloseWindow();
        }
        else
        {
            TimeText.SetText(Storage.GetPreheatLeftTimeText());   
        }
    }

    private StorageSeaRacing Storage;
    public void BindStorage(StorageSeaRacing storage)
    {
        if (storage != null)
        {
            Storage = storage;
        }
        else
        {
            Debug.LogError("预热弹窗绑定storage为null");
        }
    }
    public static UIPopupSeaRacingPreviewController Open(StorageSeaRacing storage)
    {
        var popup = UIManager.Instance.OpenUI(UINameConst.UIPopupSeaRacingPreview) as UIPopupSeaRacingPreviewController;
        popup.BindStorage(storage);
        return popup;
    }
}