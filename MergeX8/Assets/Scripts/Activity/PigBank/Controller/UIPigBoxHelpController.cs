using UnityEngine;
using UnityEngine.UI;


public class UIPigBoxHelpController : UIWindowController
{
    private Button _closeBtn;
    private bool _canClose = false;
    private Animator _animator;

    public override void PrivateAwake()
    {
        CommonUtils.NotchAdapte(transform.Find("Root") as RectTransform);

        _animator = transform.GetComponent<Animator>();

        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnClose);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        EventDispatcher.Instance.DispatchEvent(EventEnum.NOTICE_POPUP);

        StartCoroutine(CommonUtils.PlayAnimation(_animator, "appear", "", () => { _canClose = true; }
        ));
    }

    private void OnClose()
    {
        if (!_canClose)
            return;

        _canClose = false;

        StartCoroutine(CommonUtils.PlayAnimation(_animator, "disappear", "", () => { CloseWindowWithinUIMgr(true); }
        ));
    }
}