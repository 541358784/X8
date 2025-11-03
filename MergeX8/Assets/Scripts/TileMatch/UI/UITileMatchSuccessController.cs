using Decoration;
using Decoration.DynamicMap;
using Farm.Model;
using TileMatch.Event;
using UnityEngine.UI;

public partial class UITileMatchSuccessController : UIWindowController
{
    public override void PrivateAwake()
    {
        CloseBtn = transform.Find("Root/CommonPopupBG1/CloseButton").GetComponent<Button>();
        CloseBtn.onClick.AddListener(ClickContinue);
        StartBtn = transform.Find("Root/StartButton").GetComponent<Button>();
        StartBtn.onClick.AddListener(ClickContinue);
    }

    private Button CloseBtn;
    private Button StartBtn;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        
    }
    public void ClickContinue()
    {
        DecoManager.Instance.CurrentWorld.ShowByPosition();
        CloseWindowWithinUIMgr(true);
        TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_GoHome);
        if (!UIKapiTileMainController.Instance)
            DragonPlus.AudioManager.Instance.PlayMusic(1, true);
        UIKapiTileMainController.Show(true);
    }
}