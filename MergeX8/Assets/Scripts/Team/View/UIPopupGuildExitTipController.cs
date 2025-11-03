using System;
using UnityEngine.UI;

public class UIPopupGuildExitTipController:UIWindowController
{
    public override void PrivateAwake()
    {
        
    }
    
    public static UIPopupGuildExitTipController Instance;
    public static UIPopupGuildExitTipController Open(Action<bool> callback)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr();
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupGuildExitTip,callback) as UIPopupGuildExitTipController;
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