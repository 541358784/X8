using UnityEngine.UI;

public class UIPopupStarrySkyCompassNoItemController:UIWindowController
{
    private Button CloseBtn;
    private Button StartBtn;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(()=>AnimCloseWindow());
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(()=>
        {
            var mainUI = UIStarrySkyCompassMainController.Instance;
            if (mainUI)
                mainUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        });
    }

    public static UIPopupStarrySkyCompassNoItemController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupStarrySkyCompassNoItem) as
            UIPopupStarrySkyCompassNoItemController;
    }
}