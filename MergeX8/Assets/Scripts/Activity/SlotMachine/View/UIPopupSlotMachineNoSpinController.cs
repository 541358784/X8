using UnityEngine.UI;

public class UIPopupSlotMachineNoSpinController:UIWindowController
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
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIPopupSlotMachineMainController>(UINameConst.UIPopupSlotMachineMain);
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

    public static UIPopupSlotMachineNoSpinController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupSlotMachineNoSpin) as
            UIPopupSlotMachineNoSpinController;
    }
}