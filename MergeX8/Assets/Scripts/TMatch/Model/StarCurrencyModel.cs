using DragonU3DSDK.Storage;
using Framework;
using DragonPlus.Config.TMatch;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

namespace TMatch
{


    public class StarCurrencyModel : GlobalSystem<StarCurrencyModel>, IInitable
    {
        private StorageTMatch storageTMatch;

        public void Init()
        {
            storageTMatch = StorageManager.Instance.GetStorage<StorageTMatch>();
            EventDispatcher.Instance.AddEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);

        }

        public void Release()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMatchResultExecute, OnTMatchResultExecute);
        }

        private void OnTMatchResultExecute(BaseEvent evt)
        {
            TMatchResultExecuteEvent realEvt = evt as TMatchResultExecuteEvent;
            if (realEvt.ExecuteType != TMatchResultExecuteType.Star) return;
            if (!realEvt.LevelData.win)
            {
                LobbyTaskSystem.Instance.FinishCurrentTask();
                return;
            }

            var getStar = GetStarCnt(realEvt.LevelData.layoutCfg.levelTimes, realEvt.LevelData.LastTimes);
            // CurrencyModel.Instance.AddRes(ResourceId.Key, getStar, new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventMatchFrenzy.Types.ItemChangeReason.Star));
            Transform sourceTra = UILobbyMainViewLevelButton.GetTopView();
            FlySystem.Instance.FlyItem(3, getStar,
                sourceTra.position, FlySystem.Instance.GetTargetTransform(3).position,
                () => { LobbyTaskSystem.Instance.FinishCurrentTask(); });
        }

        public int GetStarCnt(float allTimes, float lastTimes)
        {
            var globalConfig = TMatchConfigManager.Instance.GlobalList[0];
            var rateConfig = globalConfig.MatchLevelWinStartRate;
            var winStarConfig = globalConfig.MatchLevelWinStartCnt;
            float timeRatio = lastTimes / allTimes * 100;
            timeRatio = timeRatio > 100 ? 100 : timeRatio; // 因为时钟的存在，时间有可能比总时间长
            var starNum = 1;
            for (var i = 0; i < rateConfig.Length; i++)
            {
                if (timeRatio <= rateConfig[i])
                {
                    starNum = winStarConfig[i];
                    break;
                }
            }

            return starNum;
        }

    }
}