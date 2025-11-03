using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;
public class UIDogStartController:UIWindowController
{
    private Button _buttonClose;
    private Button _buttonStart;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _buttonStart = GetItem<Button>("Root/Button");
        _buttonStart.onClick.AddListener(OnStartBtn);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDogPop);
        DogHopeModel.Instance.ShowStartView();
    }

    private void OnStartBtn()
    {
        AnimCloseWindow(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIDogMain);
        });
    }

    private void OnCloseBtn()
    {
       AnimCloseWindow();
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow();
    }
}
