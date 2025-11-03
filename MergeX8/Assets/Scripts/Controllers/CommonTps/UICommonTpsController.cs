using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class UICommonTpsController : UIWindowController
{
    private LocalizeTextMeshProUGUI _contentText;
    private Button _closeBtn;
    private Action _closeAction;
    public override void PrivateAwake()
    {
        _contentText = GetItem<LocalizeTextMeshProUGUI>("BgImage/Text");
        _closeBtn = GetItem<Button>("CloseButton");
        _closeBtn.onClick.AddListener(() =>
        {
            CloseWindowWithinUIMgr(true, _closeAction);
        });
    }
    
    protected override void OnOpenWindow(params object[] objs)
    {
        string contentKey = (string)objs[0];
        _closeAction =  (Action)objs[1];
        
        _contentText.SetTerm(contentKey);
    }
    
    public override void ClickUIMask()
    {
        CloseWindowWithinUIMgr(true, _closeAction);
    }

    //ui_item_recycle_special_item  海豹
    //ui_item_recycle_event_item    复活节
    public static void OpenCommonTips(string key, Action closeAction)
    {
        UIManager.Instance.OpenUI(UINameConst.UICommonTps, key, closeAction);
    }
}