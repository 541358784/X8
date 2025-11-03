using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Farm.Model;
using Framework;
using UnityEngine;


namespace TMatch
{
    public enum TMGameType
    {
        Normal,
        Kapibala,
    }
    public class FsmParamTMatch : FsmParam
    {
        public int level;
        public Layout layoutCfg;
        public TMGameType GameType;

        public FsmParamTMatch(int inlevel,Layout inlayoutCfg, TMGameType inGameType)
        {
            level = inlevel;
            layoutCfg = inlayoutCfg;
            GameType = inGameType;
        }
    }

    public class StateTMatch : IFsmState
    {
        StatusType IFsmState.Type => StatusType.TripleMatch;

        private FsmParamTMatch fsmParamTMatch;

        private TMatchSystem tMatchSystem;

        public async void Enter(params object[] objs)
        {
            // UIHomeMainController.mainController.AnimShowMainUI(false, true);
            UIHomeMainController.HideUI();
            FarmModel.Instance.AnimShow(false);
            await PreEnterAsync(objs[0] as FsmParam);
            EnterFinish();
        }
        public async Task<bool> PreEnterAsync(FsmParam param)
        {
            UIViewSystem.Instance.Open<UITMatchMainController>();
            fsmParamTMatch = param as FsmParamTMatch;
            tMatchSystem = new TMatchSystem();
            tMatchSystem.Enter(fsmParamTMatch);
            return true;
        }

        public void EnterFinish()
        {
            DragonPlus.GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmLevelStart,
                data1: TMatchModel.Instance.GetMainLevel().ToString());
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_TRY_AGAIN, OnTryAgainEvt);
            if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                
            }
            else
            {
                PlayBGMusic();   
            }
            if (fsmParamTMatch.level > 1 || fsmParamTMatch.GameType != TMGameType.Normal)
                AudioSysManager.Instance.PlaySound(SfxNameConst.Yx_Level_item);
            // DragonPlus.GameBIManager.SendEnterGameEvent(fsmParamTMatch.level); // bi
        }

        public void PlayBGMusic()
        {
            var musicPath = "Level_bgm_01";
            AudioSysManager.Instance.PlayMusic(musicPath);
        }

        public void Update(float deltaTime)
        {
            tMatchSystem?.Update(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            tMatchSystem?.LateUpdate(deltaTime);
        }

        public void Exit()
        {
            tMatchSystem.Exit();
            tMatchSystem = null;
            // UIViewSystem.Instance.CloseAll((int)UIViewLayer.Lobby, (int)UIViewLayer.Notice , (int)UIViewLayer.Loading, (int)UIViewLayer.TopNotice);
            DOTween.KillAll();
            UIViewSystem.Instance.CloseAll();
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TRY_AGAIN, OnTryAgainEvt);
        }

        public void PlayBgm()
        {
            PlayBGMusic();
        }

        // public void LateUpdate(float deltaTime)
        // {
        //     tMatchSystem.LateUpdate(deltaTime);
        // }

        // public void Exit(FsmStateType toStateType)
        // {
        //     tMatchSystem.Exit();
        //     tMatchSystem = null;
        //     // UIViewSystem.Instance.CloseAll((int)UIViewLayer.Lobby, (int)UIViewLayer.Notice , (int)UIViewLayer.Loading, (int)UIViewLayer.TopNotice);
        //     UIViewSystem.Instance.CloseAll();
        //     
        //     EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_TRY_AGAIN, OnTryAgainEvt);
        // }

        private void OnTryAgainEvt(BaseEvent evt)
        {
            SceneFsm.mInstance.ChangeStateForce(StatusType.TripleMatch, fsmParamTMatch);
        }
    }
}
