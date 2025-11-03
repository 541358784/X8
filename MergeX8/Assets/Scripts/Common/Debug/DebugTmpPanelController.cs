using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class DebugTmpPanelController : DebugTmpPanel
{
    public static readonly string AssetPath = "Common/DebugTmpPanel";
    private ContentSizeFitter sizefitter;

    public static void Open(params object[] objs)
    {
//        UIManager.Instance.OpenUI(AssetPath, objs);
        UIManager.Instance.OpenUI(UINameConst.DebugTmpPanel, objs);
    }

    public static DebugTmpPanelController Open(System.Action CloseAction = null, params object[] objs)
    {
//        DebugTmpPanelController window = UIManager.Instance.OpenUI(AssetPath, objs) as DebugTmpPanelController;
        // var window = UIManager.Instance.OpenUI<DebugTmpPanelController>(AssetPath, UIWindowLayer.Tips, UIWindowType.PopupTip, objs);
        // window.CloseAction = CloseAction;
        // return window;
        return null;
    }

    protected override void Initialize()
    {
        sizefitter = tmp_TestText.transform.parent.GetComponent<ContentSizeFitter>();

//        StartCoroutine(RefreshText());
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        CommonUtils.TweenOpen(transform);
        //UIMainGroupController.AnimHide();
        tmp_TestText.SetText((string) objs[0]);
    }

    private IEnumerator RefreshText()
    {
        yield return new WaitForSeconds(0.5f);
        sizefitter.enabled = false;
        yield return new WaitForEndOfFrame();
        sizefitter.enabled = true;
        yield return new WaitForEndOfFrame();
    }

    protected override void OnHide()
    {
    }

    private void OnDestroy()
    {
        //UIMainGroupController.TryAnimShow(this);
    }

    public override void CloseBtnClicked()
    {
        base.CloseButtonClick();
    }
}