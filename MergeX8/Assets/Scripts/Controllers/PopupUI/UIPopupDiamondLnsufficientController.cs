using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
public class UIPopupDiamondLnsufficientController : UIWindowController
{
    private Button ButtonPlayButton { get; set; }
    private Button ButtonClose { get; set; }
    private Action _action;

    private LocalizeTextMeshProUGUI _text;
    private LocalizeTextMeshProUGUI _num;
    public override void PrivateAwake()
    {
        ButtonPlayButton = transform.Find("Root/Button").GetComponent<Button>();
        ButtonPlayButton.onClick.AddListener(OnButtonPlayButtonClick);
        
        ButtonClose = transform.Find("Root/ButtonClose").GetComponent<Button>();
        ButtonClose.onClick.AddListener(()=>
        {
            AnimCloseWindow();
        });

        _text = transform.Find("Root/BubbleGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _num = transform.Find("Root/BubbleGroup/Num").GetComponent<LocalizeTextMeshProUGUI>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        if (objs != null && objs.Length > 1)
        {
            _action = objs[0] as Action;
            _text.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_deficient_gmes_desc"),
                Mathf.Abs(UserData.Instance.GetRes(UserData.ResourceId.Diamond)-(int)objs[1])));
            _num.SetText(objs[1].ToString());
        }

    }

    private void OnButtonPlayButtonClick()
    {
        AnimCloseWindow(() =>
        {
            _action?.Invoke();
        });
    }

}