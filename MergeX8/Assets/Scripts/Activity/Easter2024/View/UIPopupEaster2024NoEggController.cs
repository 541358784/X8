using UnityEngine.UI;

public class UIPopupEaster2024NoEggController:UIWindowController
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
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIEaster2024MainController>(UINameConst.UIEaster2024Main);
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

    public static UIPopupEaster2024NoEggController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupEaster2024NoEgg) as
            UIPopupEaster2024NoEggController;
    }
}