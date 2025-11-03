using System;
using Decoration;
using Decoration.DynamicMap;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Farm.Model;
using Screw;
using Screw.GameLogic;
using Screw.Module;

public partial class UIKapiScrewMainController
{
    public SceneFsmScrewGame _subScrewGame;

    public void ExitGameOnFail()
    {
        FarmModel.Instance.AnimShow(true);
        UIHomeMainController.ShowUI();
        PlayerManager.Instance.RecoverPlayer();
        DecoManager.Instance.CurrentWorld.ShowByPosition();
        DynamicMapManager.Instance.PauseLogic = false;
        UIRoot.Instance.EnableEventSystem = true;
        gameObject.SetActive(true);
        
        _subScrewGame.Exit();
        KapiScrewModel.Instance.DealFail();
        PerformOnGameFail();
    }
    public async void OnClickStartBtn()
    {
        if (KapiScrewModel.Instance.GetLife() <= 0)
        {
            UIPopupKapiScrewShopController.Open();
            return;
        }
        if (Storage.ChangeEnemy)
            return;
        CloseBtn.interactable = false;
        StartBtn.interactable = false;
        if (!LoopPlayBall)
        {
            StartLoopPlayBall();
            await XUtility.WaitSeconds(1f);
        }
        KapiScrewModel.Instance.DealStartGame();
        var curLevel = KapiScrewModel.Instance.GetLevelConfig(Storage.BigLevel);
        var screwLevelId = curLevel.SmallLevels[Storage.PlayingSmallLevel];
        Action<object> action = (b) =>
        {
            var type = (ScrewGameKapiScrewContext.ScrewGameCallbackType)b;
                
            if (type == ScrewGameKapiScrewContext.ScrewGameCallbackType.Win)
            {
                FarmModel.Instance.AnimShow(true);
                UIHomeMainController.ShowUI();
                PlayerManager.Instance.RecoverPlayer();
                DecoManager.Instance.CurrentWorld.ShowByPosition();
                DynamicMapManager.Instance.PauseLogic = false;
                UIRoot.Instance.EnableEventSystem = true;
                gameObject.SetActive(true);
                
                _subScrewGame.Exit();
                KapiScrewModel.Instance.DealWin();
                PerformOnGameWin();
            }
            else if (type == ScrewGameKapiScrewContext.ScrewGameCallbackType.FullFail)
            {
                UIModule.Instance.ShowUI(typeof(UIKapiScrewKeepPlayingPopup), ScrewGameLogic.Instance.context);
            }
            else if (type == ScrewGameKapiScrewContext.ScrewGameCallbackType.BlockerFail)
            {
                // _subScrewGame.RePlay(false);   
                UIModule.Instance.ShowUI(typeof(UIKapiScrewBlockerFailedPopup), ScrewGameLogic.Instance.context);
            }
        };

        TMatch.UILoadingEnter.Open(() =>
        {
            FarmModel.Instance.AnimShow(false);
            UIHomeMainController.HideUI(false, true);
            PlayerManager.Instance.HidePlayer();
            DecoManager.Instance.CurrentWorld.HideByPosition();
            DynamicMapManager.Instance.PauseLogic = true;
            gameObject.SetActive(false);
            
            _subScrewGame = new SceneFsmScrewGame();
            _subScrewGame.Enter(screwLevelId, action,false);
            StartBtn.interactable = true;
            CloseBtn.interactable = true;
        });
    }
}