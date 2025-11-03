using System;
using UnityEngine.UI;

public class UIPopupGuildKickTipController:UIWindowController
{
    public override void PrivateAwake()
    {
        
    }
    
    public static UIPopupGuildKickTipController Instance;
    public static UIPopupGuildKickTipController Open(Action<bool> callback)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildKickTip,callback) as UIPopupGuildKickTipController;
        return Instance;
    }

    private Action<bool> Callback;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Callback = objs[0] as Action<bool>;
        transform.Find("Root/ButtonGroup/ButtonNo").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
            Callback?.Invoke(false);
        });
        transform.Find("Root/ButtonGroup/ButtonYes").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
            Callback?.Invoke(true);
        });
    }
}