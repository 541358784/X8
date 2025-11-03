using UnityEngine;
using UnityEngine.UI;

public abstract class DebugTmpPanel : UIWindow
{
    public System.Action CloseAction;
    protected DragonPlus.LocalizeTextMeshProUGUI tmp_TestText;
    protected Image img_Viewport;
    protected Image img_Handle;
    protected Image img_ScrollbarHorizontal;
    protected Image img_Handle_1;
    protected Image img_ScrollbarVertical;
    protected Image img_ScrollView;
    protected Image img_BaseBoard;
    protected CanvasGroup cgp_Root;
    protected Button btn_CloseBtn;

    public override void PrivateAwake()
    {
        tmp_TestText = transform.Find("Root/BaseBoard/Scroll View/Viewport/Content/TestText")
            .GetComponent<DragonPlus.LocalizeTextMeshProUGUI>();
        img_Viewport = transform.Find("Root/BaseBoard/Scroll View/Viewport").GetComponent<Image>();
        img_Handle = transform.Find("Root/BaseBoard/Scroll View/Scrollbar Horizontal/Sliding Area/Handle")
            .GetComponent<Image>();
        img_ScrollbarHorizontal =
            transform.Find("Root/BaseBoard/Scroll View/Scrollbar Horizontal").GetComponent<Image>();
        img_Handle_1 = transform.Find("Root/BaseBoard/Scroll View/Scrollbar Vertical/Sliding Area/Handle")
            .GetComponent<Image>();
        img_ScrollbarVertical = transform.Find("Root/BaseBoard/Scroll View/Scrollbar Vertical").GetComponent<Image>();
        img_ScrollView = transform.Find("Root/BaseBoard/Scroll View").GetComponent<Image>();
        img_BaseBoard = transform.Find("Root/BaseBoard").GetComponent<Image>();
        cgp_Root = transform.Find("Root").GetComponent<CanvasGroup>();
        btn_CloseBtn = transform.Find("CloseBtn").GetComponent<Button>();

        btn_CloseBtn.onClick.AddListener(CloseBtnClicked);

        Initialize();
    }

    public void CloseButtonClick()
    {
        CommonUtils.TweenClose(transform, () =>
        {
            CloseWindowWithinUIMgr(true);
            CloseAction?.Invoke();
        });
    }


    protected override void OnCloseWindow(bool destroy = false)
    {
        if (destroy)
        {
            btn_CloseBtn.onClick.RemoveAllListeners();
        }
        else
        {
            OnHide();
        }

        base.OnCloseWindow(destroy);
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void OnHide()
    {
    }

    public abstract void CloseBtnClicked();
}
/*
using UnityEngine;
/// <summary>
/// Assets/Export/Prefabs/UI/Cooking/Common/DebugTmpPanel.prefab
/// </summary>
public class DebugTmpPanelController : DebugTmpPanel {
    public static readonly string AssetPath = "Common/DebugTmpPanel";
    public static void Open(params object[] objs)
    {
        UIManager.Instance.OpenUI(AssetPath, objs);
        // UIManager.Instance.OpenCookingWindow(AssetPath, UIWindowLayer.Tips, UIWindowType.PopupTip, objs);
    }
    public static DebugTmpPanelController Open(System.Action CloseAction= null,params object[] objs)
    {
        DebugTmpPanelController window = UIManager.Instance.OpenUI(AssetPath, objs) as DebugTmpPanelController;
        // var window = UIManager.Instance.OpenCookingWindow<DebugTmpPanelController>(AssetPath, UIWindowLayer.Tips, UIWindowType.PopupTip, objs);
        window.CloseAction = CloseAction;
        return window;
    }
    protected override void Initialize(){
        
    }
    protected override void OnOpenWindow (params object[] objs){
        CommonUtils.TweenOpen(transform);
    }
    protected override void OnHide (){

    }
    private void OnDestroy(){

    }
    public override void CloseBtnClicked(){}

}
*/