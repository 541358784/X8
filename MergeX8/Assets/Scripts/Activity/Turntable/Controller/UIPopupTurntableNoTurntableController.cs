using UnityEngine.UI;

public class UIPopupTurntableNoTurntableController: UIWindowController
{
    public override void PrivateAwake()
    {
        var closeBtn = GetItem<Button>("Root/ButtonClose");
        closeBtn.onClick.AddListener(()=>AnimCloseWindow());
        var statBtn = GetItem<Button>("Root/Button");
        statBtn.onClick.AddListener(()=>
        {
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIPopupTurntableMainController>(UINameConst.UIPopupTurntableMain);
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
}