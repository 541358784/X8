using System;
using UnityEngine.UI;

public class UIPopupBiuBiuTipController : UIWindowController
{
    public static UIPopupBiuBiuTipController Instance;
    public static UIPopupBiuBiuTipController Open(Action callback)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupBiuBiuTip,callback) as UIPopupBiuBiuTipController;
        return Instance;
    }

    private Action Callback;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Callback = objs[0] as Action;
    }
    
    
    private Button _playBtn;
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _playBtn = GetItem<Button>("Root/Button");
        _playBtn.onClick.AddListener(OnPlayBtn);
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnCloseBtn);
    
    }

    public void OnPlayBtn()
    {
        AnimCloseWindow(Callback);
    }

    public void OnCloseBtn()
    {
        AnimCloseWindow(Callback);
    }
}