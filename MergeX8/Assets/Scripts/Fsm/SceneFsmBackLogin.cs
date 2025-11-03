using Decoration;
using Decoration.Bubble;
using Decoration.WorldFogManager;
using DG.Tweening;
using DragonU3DSDK.Asset;
using Farm.Model;
using Framework;
using Game;
using UnityEngine;

public class SceneFsmBackLogin : IFsmState
{
    public StatusType Type => StatusType.BackLogin;

    public void Enter(params object[] objs)
    {
        SceneFsm.mInstance.ClientLogin = false;
        CoroutineManager.Instance.StopAllCoroutines();

        PlayerManager.Instance.UnLoad();
        FarmModel.Instance.Release();
        AudioMusicLogic.Instance.StopMusic();
        ShieldButtonManager.Instance.Destroy();
        DOTween.KillAll(true);
        UIManager.Instance.CloseUI(UINameConst.UIStore);
        UIHomeMainController.mainController = null;
        UIManager.Instance.ClearAllUI();
        AnimControlManager.Instance.Destory();
        ResourcesManager.Instance.UnLoadAllCache();
        DownloadManager.Instance.AbortAllDownloadTask();
        LoadingController.HideLoading();
        CameraManager.MainCamera?.gameObject.SetActive(false);
        DecoManager.Instance.CleanAll();
        GuideSubSystem.Instance.CloseCurrent();
        GuideSubSystem.Instance.CleanGuide();
        WorldFogManager.Instance.UnLoad();
        NodeBubbleManager.Instance.UnLoadBubble();
        // UIManager.Instance.CloseUI(UIPath.UIGameMain);
        // GameObject.Destroy(M3GameManager.gameScreen);
        UIManager.Instance.OpenUI(UINameConst.UILogin);
        UIManager.Instance.SetCanvasGroupAlpha(true, false);
        EventDispatcher.Instance.DispatchEvent(EventEnum.BackLogin);
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