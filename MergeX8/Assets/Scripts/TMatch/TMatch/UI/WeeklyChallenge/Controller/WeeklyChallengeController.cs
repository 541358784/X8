using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Dlugin;
using DragonPlus.Config.TMatch;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Network.API;
// using EventTMatch;
using Framework;
using OutsideGuide;
using UnityEngine;
using ActivityWeeklyChallengeModel = WeeklyChallengeModel;

namespace TMatch
{
    public class WeeklyChallengeController : GlobalSystem<WeeklyChallengeController>, IInitable
    {
        public bool alreadySyncWithServer;

        public WeeklyChallengeModel model;

        public int CurCollectItemId;

        public void Init()
        {
            InitModel();
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_CREATE, OnGameCreateEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_START, OnGameStartEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_EVENT_UPDATE, InitModel);
        }

        private void InitModel(BaseEvent obj)
        {
            InitModel();
        }

        public void Release()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_CREATE, OnGameCreateEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_START, OnGameStartEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_EVENT_UPDATE, InitModel);
        }

        public void InitModel()
        {
            if (model != null || !IsOpen())
                return;

            model = new WeeklyChallengeModel();
            SyncWithServer();
        }

        public bool IsOpen()
        {
            if (!ActivityWeeklyChallengeModel.Instance.IsInitFromServer())
                return false;
            
            var cfgList = DragonPlus.Config.WeeklyChallenge.WeeklyChallengeConfigManager.Instance.EventWeeklyChallengeList;
            
            var firstWeekCfg = cfgList?.Find(x => x.week == 1);
            if (firstWeekCfg == null)
                return false;
            
            if ((long)APIManager.Instance.GetServerTime() < firstWeekCfg.starTimeSec.ToLong())
                return false;
            
            return true;
        }

        private void OnGameCreateEvt(BaseEvent evt)
        {
            if(model == null)
                return;
            
            if (!model.IsUnlock() || model.IsClaimedAll() || !alreadySyncWithServer) return;
            WeeklyChallenge weeklyChallengeCfg = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg();
            List<int> ItemRandomCnt = UnlimitItemModel.Instance.UnlimitedItemLeftTime(ItemType.TMWeeklyChallengeBuff) > 0 ? weeklyChallengeCfg.collectItemRandomCntBuff.ToList() : weeklyChallengeCfg.collectItemRandomCnt.ToList();
            int cnt = ItemRandomCnt[Random.Range(0, ItemRandomCnt.Count)];
            if (cnt == 0)
            {
                if (WeeklyChallengeController.Instance.model.IsUnlock() && !DecoGuideManager.Instance.GetGuideState(10122))
                    cnt = 3;
                else return;
            }

            var cfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallengeCfg.collectItemId);
            for (int i = 0; i < cnt; i++)
            {
                TMatchItemSystem.Instance.Create(cfg.subId);
            }

            EventDispatcher.Instance.DispatchEvent(new TMatchNeedCollectItemEvent(cfg.subId));
        }

        private async void OnGameStartEvt(BaseEvent evt)
        {
            if(model == null)
                return;
            
            await Task.Delay(1000);
            if (!model.IsUnlock() || model.IsClaimedAll() || !alreadySyncWithServer) return;

            if (!WeeklyChallengeController.Instance.model.IsUnlock() || DecoGuideManager.Instance.GetGuideState(10122))
                return;

            WeeklyChallenge weeklyChallengeCfg = model.GetCurWeeklyChallengeCfg();
            var cfg = TMatchShopConfigManager.Instance.GetItem(weeklyChallengeCfg.collectItemId);
            CurCollectItemId = cfg.subId;
            TMatchBaseItem baseItem = TMatchItemSystem.Instance.Create(cfg.subId);
            Vector3 min = TMatchEnvSystem.Instance.SceneRandomPosMin + new Vector3(1.0f, 0.0f, 1.0f);
            Vector3 max = TMatchEnvSystem.Instance.SceneRandomPosMax + new Vector3(-1.0f, 0.0f, -1.0f);
            baseItem.RandomPos(min, max);
            TMatchItemSystem.Instance.Explode(baseItem.GameObject.transform.position);
            // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchItemWeeklyChallenge, baseItem.GameObject.transform);
        }

        private async UniTaskVoid SyncWithServer()
        {
            while (!destory && !alreadySyncWithServer)
            {
                alreadySyncWithServer = APIManager.Instance.GetLastSyncServerTime() > 0;
                await UniTask.WaitForSeconds(1);
            }

            if(TMatchConfigManager.Instance.GlobalList == null)
                TMatchConfigManager.Instance.InitConfig();
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.WeeklyChallengeStateReset);
            while (!destory && TMatchModel.Instance.GetMainLevel() < TMatchConfigManager.Instance.GlobalList[0].WeeklyChallengeUnlcok)
            {
                await UniTask.WaitForSeconds(1);
            }

            if (!destory) CheckStarNextWeek();
        }

        private async void CheckStarNextWeek()
        {
            if(model == null)
                return;
            
            long leftTime = model.GetCurWeekLeftTime();
            // Debug.Log($"weekly 1 : {leftTime}");
            // Debug.Log($"weekly 2 : {(int)leftTime + 1000}");

            if (leftTime > 0 && leftTime <= 604800000 /*七天*/) await Task.Delay((int)leftTime + 1000); // 加1000毫秒规避绝大部分的时间波动
            model.StarNextWeek();
            EventDispatcher.Instance.DispatchEvent(EventEnum.WeeklyChallengeStateReset);
            await Task.Yield(); //避免卡死
            CheckStarNextWeek();
        }

        public bool TryOpenView()
        {
            if (WeeklyChallengeGateView.GateStateType != WeeklyChallengeGateView.StateType.Play) return false;
            if (WeeklyChallengeController.Instance.model.stoage.Popup) return false;
            if (TMBPModel.Instance.GameEndWellPopMainView()) return false;
            WeeklyChallengeController.Instance.model.stoage.Popup = true;
            UIViewSystem.Instance.Open<WeeklyChallengeView>();
            return true;
        }
    }
}