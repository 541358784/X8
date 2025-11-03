using UnityEngine.UI;

public class UIPopupRemoveGuideController : UIWindowController
{
    public override void PrivateAwake()
    {   
        transform.Find("Root/CommonPopupBG1/CloseButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        
        transform.Find("Root/Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
    }
}