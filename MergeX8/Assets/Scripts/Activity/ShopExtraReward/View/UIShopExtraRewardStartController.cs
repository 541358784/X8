using System;
using DragonPlus;
using UnityEngine.UI;

public class UIPopupShopExtraRewardStartController:UIWindowController
{
    private LocalizeTextMeshProUGUI TimeText;
    private Button StartBtn;
    private Button CloseBtn;
    public override void PrivateAwake()
    {
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(OnClickStartBtn);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,1f);
        EventDispatcher.Instance.AddEvent<EventShopExtraRewardEnd>(OnActivityEnd);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventShopExtraRewardEnd>(OnActivityEnd);
    }

    public void OnActivityEnd(EventShopExtraRewardEnd evt)
    {
        AnimCloseWindow();
    }
    public void OnClickStartBtn()
    {
        AnimCloseWindow(() =>
        {
            UIStoreController.OpenUI("ShopExtraRewardStartPopup",ShowArea.gem_shop); 
        });
    }
    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public void UpdateTime()
    {
        TimeText.SetText(ShopExtraRewardModel.Instance.GetActivityLeftTimeString());
    }
    public static UIPopupShopExtraRewardStartController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupShopExtraRewardStart) as UIPopupShopExtraRewardStartController;
    }
}