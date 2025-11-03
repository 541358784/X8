using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus.Config.TMatchShop;
using Framework;
using Newtonsoft.Json;
using UnityEngine;


namespace TMatch
{
    public class FsmParamTMatchEntry : FsmParam
    {
        public StatusType FromStateType = StatusType.Non;

        public bool FirstBindRewardPendingApple;
        public bool FirstBindRewardPendingFacebook;

        public TMatchLevelData lastMatchLevelData;
        public List<ItemData> lastMakeoverData;
        public List<ItemData> lastASMRData;

        public FsmParamTMatchEntry(StatusType fromStateType)
        {
            FromStateType = fromStateType;
        }
    }

    public class StateTMatchEntry : IFsmState
    {
        private FsmParamTMatchEntry _param;
        StatusType IFsmState.Type => StatusType.TripleMatchEntry;

        public async void Enter(params object[] objs)
        {
            await PreEnterAsync(objs[0] as FsmParam);
            EnterFinish();
        }
        public async Task<bool> PreEnterAsync(FsmParam param)
        {
            _param = param as FsmParamTMatchEntry;

            if (_param is { FromStateType: StatusType.Home })
            {
                var tempData = PlayerPrefs.GetString("TMatchLevelResultData");
                if (!string.IsNullOrEmpty(tempData))
                {
                    var levelData = JsonConvert.DeserializeObject<TMatchLevelData>(tempData);
                    if (levelData != null && levelData.level + 1 == TMatchModel.Instance.GetMainLevel())
                    {
                        _param.lastMatchLevelData = levelData;
                    }
                }
            }

            UIViewSystem.Instance.Open<UILobbyView>();

            LobbyTaskSystem.Instance.Star(new LobbyTaskParam()
            {
                MatchLevelData = _param?.lastMatchLevelData,
                ASMRLevelData = _param?.lastASMRData,
            });

            TMatchShopConfigManager.Instance.InitBoost();
            return true;
        }

        public void EnterFinish()
        {
            PlayBgm();
        }

        public void Update(float deltaTime)
        {
        }

        public void LateUpdate(float lateUpdateCount)
        {
        }

        public void Exit()
        {
            DOTween.KillAll(true);
            UIViewSystem.Instance.CloseAll();
        }

        public void PlayBgm()
        {
            var musicPath = "Home_bgm_03";
            AudioSysManager.Instance.PlayMusic(musicPath);
        }
    }
}
