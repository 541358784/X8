using Activity.TreasureHuntModel;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
public class UITreasureHuntHelpController : UIWindowController
{
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnCLose);
    
    }
    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }

}
