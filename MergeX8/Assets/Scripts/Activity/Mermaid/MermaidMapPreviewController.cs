using UnityEngine;
using UnityEngine.UI;

public class MermaidMapPreviewController : UIWindowController
{
    public override void PrivateAwake()
    {
        var closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        closeButton.onClick.AddListener(() =>
        {
            AnimCloseWindow(() =>
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.NODE_PREVIEW_END);
                UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidMain);
            });
        });
    }
}