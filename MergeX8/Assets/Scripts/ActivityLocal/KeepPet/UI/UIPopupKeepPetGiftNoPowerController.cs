using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKeepPetGiftNoPowerController:UIWindowController
{
    private Button CloseBtn;
    private Button StartBtn;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(()=>AnimCloseWindow());
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(()=>
        {
            var searchUI = UIManager.Instance.GetOpenedUIByPath<UIPopupKeepPetPatrolController>(UINameConst.UIPopupKeepPetPatrol);
            if (searchUI)
                searchUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                UIPopupKeepPetTaskController.Open();
            });
        });
    }

    public static UIPopupKeepPetGiftNoPowerController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetGiftNoPower) as
            UIPopupKeepPetGiftNoPowerController;
    }
}