using System;
using DragonU3DSDK.Storage;
using Framework;
using TMatch;
public class CrocodileController : Singleton<CrocodileController>
{
    public void Init()
    {
        Release();

        TMatch.EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.TMATCH_GAME_START, onGameStart);
        TMatch.EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.TMATCH_GAME_WIN, OnGameWinEvt);
        TMatch.EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.TMATCH_GAME_FAIL, OnGameFailedEvt);
        TMatch.EventDispatcher.Instance.AddEventListener(TMatch.EventEnum.TMATCH_GAME_EXIT, OnGameFailedEvt);
    }

    public void Release()
    {
        TMatch.EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.TMATCH_GAME_START, onGameStart);
        TMatch.EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.TMATCH_GAME_WIN, OnGameWinEvt);
        TMatch.EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.TMATCH_GAME_FAIL, OnGameFailedEvt);
        TMatch.EventDispatcher.Instance.RemoveEventListener(TMatch.EventEnum.TMATCH_GAME_EXIT, OnGameFailedEvt);
    }

    public void onGameStart(TMatch.BaseEvent evt)
    {
        CrocodileActivityModel.Instance.OnGameStart();
    }

    private void OnGameWinEvt(TMatch.BaseEvent evt)
    {
        CrocodileActivityModel.Instance.OnGameWin();
    }

    private void OnGameFailedEvt(TMatch.BaseEvent evt)
    {
        CrocodileActivityModel.Instance.OnGameFailed();
    }
}