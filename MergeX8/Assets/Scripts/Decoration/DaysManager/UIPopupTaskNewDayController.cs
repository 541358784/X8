using System;
using UnityEngine;

public class UIPopupTaskNewDayController : UIWindowController
{
    private Action _closeAction;
    public override void PrivateAwake()
    {
        _animator = transform.GetComponent<Animator>();

        StartCoroutine(CommonUtils.PlayAnimation(_animator, "appear", "", () =>
        {
            CloseWindowWithinUIMgr(true);
            _closeAction?.Invoke();
        }));
    }
    
    protected override void OnOpenWindow(params object[] objs)
    {
        _closeAction = objs[0] as Action;
    }
    
    public override void ClickUIMask()
    {
    }
}