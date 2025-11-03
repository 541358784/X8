using Activity.FarmTimeOrder;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Account;
using Framework;
using Gameplay;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class SceneFsmLogin : IFsmState
{
    public StatusType Type => StatusType.Login;

    public void Enter(params object[] objs)
    {
        WaitingManager.Instance.OpenWindow(10.0f, 0.5f, () =>
        {
            LoadingController.HideLoading();
            UIManager.Instance.OpenUI(UINameConst.UILogin);
        });
        
        UILoginController.RunOnce();
        AccountManager.Instance.Login(OnLoginResult, () =>
        {
            WaitingManager.Instance.CloseWindow();
            LoadingController.HideLoading();
            UIManager.Instance.OpenUI(UINameConst.UILogin);
        });
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BackLogin);
    }

    private void OnLoginResult(bool loginResult)
    {
        if (loginResult)
        {
            UIPopupSaveProgressController.SendLoginEvent();
        }

        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1f, () =>
        {
            MyMain.Game.InitManager();
            SceneFsm.mInstance.EnterGame();
            WaitingManager.Instance.CloseWindow();
            MergeManager.Instance.SendMergeBoardBI(MergeBoardEnum.Main);
            
            FarmTimeLimitOrderModel.Instance.CheckJoinEnd();
            AdLocalConfigHandle.Instance.TryInitLastLoginTime();
            AdSubSystem.Instance.LoginRefreshData();
        }));
    }
    public void Update(float deltaTime)
    {
    }
    public void LateUpdate(float deltaTime)
    {
        
    }
    public void Exit()
    {
        DOTween.KillAll(true);
    }
}