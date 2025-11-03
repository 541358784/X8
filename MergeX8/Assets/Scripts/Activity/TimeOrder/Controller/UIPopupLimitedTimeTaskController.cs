
using Activity.TimeOrder;
using DragonPlus;
using DragonPlus.Config.TimeOrder;
using UnityEngine.UI;

public class UIPopupLimitedTimeTaskController : UIWindowController
{
    private Button _closeButton;
    
    public override void PrivateAwake()
    {
        _closeButton = transform.Find("Root/Button").GetComponent<Button>();
        _closeButton.onClick.AddListener(()=>
        {
            AnimCloseWindow();
        });

        var text = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        text.SetText("");

        if (TimeOrderModel.Instance.IsOpened())
        {
            var time = TimeOrderConfigManager.Instance.TableTimeOrderSettingList[0].OpenTime * 60 * 1000;
            text.SetText( CommonUtils.FormatLongToTimeStr(time));
        }
    }
}