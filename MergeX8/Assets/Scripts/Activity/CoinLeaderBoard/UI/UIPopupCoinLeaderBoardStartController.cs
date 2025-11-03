using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;

public class UICoinLeaderBoardStartController : UIWindowController
{
    private Button _buttonPlay;
    public override void PrivateAwake()
    {
        _buttonPlay = GetItem<Button>("Root/Button");
        _buttonPlay.onClick.AddListener(OnPlayBtn);
    }

    private bool isHelp = false;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs.Length > 0)
        {
            isHelp = true;
        }
    }

    public void OnPlayBtn()
    {
        if (isHelp)
        {
            AnimCloseWindow();
        }
        else
        {
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCoinLeaderBoardOn);
            CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek.IsStart = true;
            AnimCloseWindow(()=>CoinLeaderBoardModel.OpenMainPopup(CoinLeaderBoardModel.Instance.CurStorageCoinLeaderBoardWeek));   
        }
    }
}